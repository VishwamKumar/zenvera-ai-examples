# Actions artifact retention

The `Artifact retention` workflow keeps at most **three successful default-branch deployment releases** (`smarterasp-<40-character commit SHA>`) and **three coverage reports** (`coverage-report`) per repository. Coverage reports are counted across all branches, including pull requests, separately from releases.

Cleanup runs after the current artifact-producing CI workflows complete, every Monday at 06:23 UTC, and manually. Add new producer workflow names to `workflow_run.workflows` when adding CI workflows; the scheduled/manual cleanup covers all matching artifacts regardless of producer name.

Run **Actions > Artifact retention > Run workflow** with `dry_run: true` first. Review the keep/delete IDs in the log. Run again with `dry_run: false` to apply the initial cleanup. Automatic completion/scheduled runs apply cleanup. Deletion is permanent.

Coverage uploads expire after seven days. Deployment expiry stays at the repository's existing seven or fourteen days. Expiry may leave fewer than three artifacts; this policy does not guarantee indefinite rollback availability. All artifact pages are read before deletion, and artifacts are sorted by creation time with ID as a tie-breaker.

Active producers are protected. Only completed successful default-branch releases from this repository count toward release retention. Existing release pruning and deployment-dropdown refresh jobs remain authoritative for deployment choices. A missing artifact (404) during deletion is harmless; other API errors fail the cleanup visibly.

Docker build records, browser diagnostics, other artifacts, package/container images, caches, workflow runs and logs are outside this policy. Repositories without matching artifacts perform no deletions. No custom credentials or secrets are needed: cleanup uses the repository-scoped `GITHUB_TOKEN` with `actions: write`, executes only inline trusted workflow code, and never checks out PR code or downloads artifacts.

Validate the pruning contract locally with:

```sh
node scripts/ci/test-artifact-retention.cjs
```
