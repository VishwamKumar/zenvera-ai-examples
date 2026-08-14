namespace Zenvera.AiExamples.Mcp.StdioHost.Tools;

internal sealed class RandomNumberTools
{
    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")] int min = 0,
        [Description("Maximum value (exclusive)")] int max = 100)
        => Random.Shared.Next(min, max);
}
