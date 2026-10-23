// Contract tests execute the exact inline script shipped in the workflow.
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const workflow = fs.readFileSync(path.join(__dirname, '../../.github/workflows/artifact-retention.yml'), 'utf8').replace(/\r\n/g, '\n');
const script = workflow.split('          script: |\n')[1].split('\n').map(line => line.slice(12)).join('\n');
const execute = new (Object.getPrototypeOf(async function() {}).constructor)('github', 'context', 'core', script);
const artifact = (id, name = 'coverage-report', extra = {}) => ({
  id, name, created_at: new Date(Date.UTC(2026, 9, 1, 0, id)).toISOString(),
  size_in_bytes: 100, workflow_run: {id}, expired: false, ...extra
});
const release = id => artifact(id, 'smarterasp-' + id.toString(16).padStart(40, '0'));
async function run(artifacts, {dry = false, overrides = {}, deleteStatus, runError = false} = {}) {
  const deleted = [], logs = [];
  const summary = {addHeading() {return this;}, addRaw() {return this;}, async write() {}};
  const endpoint = () => {};
  const github = {
    async paginate(method, args) {
      assert.equal(method, endpoint);
      assert.equal(args.per_page, 100);
      return artifacts;
    },
    rest: {actions: {
      listArtifactsForRepo: endpoint,
      async getWorkflowRun({run_id}) {
        if (runError) throw new Error('API unavailable');
        return {data: {status: 'completed', conclusion: 'success', head_branch: 'main',
          event: 'push', head_repository: {full_name: 'owner/repo'}, ...overrides[run_id]}};
      },
      async deleteArtifact({artifact_id}) {
        deleted.push(artifact_id);
        if (deleteStatus) throw Object.assign(new Error('delete failure'), {status: deleteStatus});
      }
    }}
  };
  await execute(github, {repo: {owner: 'owner', repo: 'repo'},
    eventName: dry ? 'workflow_dispatch' : 'workflow_run',
    payload: {inputs: {dry_run: 'true'}, repository: {default_branch: 'main'}}},
    {info: line => logs.push(JSON.parse(line)), summary});
  return {deleted, logs};
}
(async () => {
  let count = 0;
  const test = async (name, callback) => {await callback(); count++; console.log('PASS ' + name);};
  await test('fewer than three are preserved', async () => {
    assert.deepEqual((await run([artifact(1), artifact(2)])).deleted, []);
  });
  await test('release and coverage limits are independent; input order is irrelevant', async () => {
    const data = [artifact(2), release(7), artifact(5), release(6), artifact(1), release(10),
      artifact(4), release(8), artifact(3), release(9)];
    assert.deepEqual((await run(data)).deleted.sort((a,b) => a-b), [1,2,6,7]);
  });
  await test('unrelated, expired and active artifacts are protected', async () => {
    const data = [artifact(1, 'browser-regressions'), artifact(2, 'x.dockerbuild'),
      artifact(3, 'coverage-report', {expired:true}), artifact(4),
      ...[5,6,7].map(id=>artifact(id))];
    assert.deepEqual((await run(data, {overrides:{4:{status:'in_progress'}}})).deleted, []);
  });
  await test('failed, PR and foreign releases cannot evict successful default-branch releases', async () => {
    const data = [1,2,3,4,5,6,7].map(release);
    const overrides = {4:{conclusion:'failure'},5:{head_branch:'feature'},
      6:{event:'pull_request'},7:{head_repository:{full_name:'fork/repo'}}};
    assert.deepEqual((await run(data,{overrides})).deleted, []);
  });
  await test('coverage from failed runs and PRs counts repository-wide', async () => {
    const result = await run([1,2,3,4,5].map(id=>artifact(id)),
      {overrides:{4:{conclusion:'failure'},5:{event:'pull_request',head_branch:'feature'}}});
    assert.deepEqual(result.deleted, [2,1]);
  });
  await test('dry run logs exact candidates without deletion', async () => {
    const result = await run([1,2,3,4].map(id=>artifact(id)), {dry:true});
    assert.deepEqual(result.deleted, []);
    assert.deepEqual(result.logs.filter(x=>x.action==='would-delete').map(x=>x.id), [1]);
  });
  await test('all paginated artifacts participate and equal timestamps use ID', async () => {
    const data = Array.from({length:105}, (_,i)=>artifact(i+1, 'coverage-report',
      {created_at:'2026-10-01T00:00:00Z'}));
    const result = await run(data);
    assert.equal(result.deleted.length,102);
    assert.deepEqual(result.logs.filter(x=>x.action==='keep').map(x=>x.id),[105,104,103]);
  });
  await test('404 deletion races are harmless; other failures surface', async () => {
    await run([1,2,3,4].map(id=>artifact(id)), {deleteStatus:404});
    await assert.rejects(run([1,2,3,4].map(id=>artifact(id)), {deleteStatus:403}), /delete failure/);
    await assert.rejects(run([1,2,3,4].map(id=>artifact(id)), {runError:true}), /API unavailable/);
  });
  console.log(count + ' retention contract tests passed');
})().catch(error => {console.error(error); process.exitCode = 1;});
