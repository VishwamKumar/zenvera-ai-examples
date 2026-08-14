namespace Zenvera.AiExamples.Shared.Hosting;

public static class ExampleConfiguration
{
    public static IConfigurationRoot Load(string userSecretsId)
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets(userSecretsId)
            .AddEnvironmentVariables()
            .Build();
    }
}
