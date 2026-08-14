namespace Zenvera.AiExamples.Mcp.StdioHost.Tools;

internal sealed class WeatherTools
{
    private static readonly string[] Conditions =
    [
        "Sunny", "Partly Cloudy", "Cloudy", "Overcast", "Light Rain",
        "Heavy Rain", "Snow", "Fog", "Windy", "Stormy"
    ];

    [McpServerTool]
    [Description("Gets current weather for a specified city.")]
    public Task<string> GetCurrentWeather(
        [Description("Name of the city to get weather for")] string city)
    {
        var payload = new
        {
            City = city,
            Temperature = Random.Shared.Next(-10, 35) + "°C",
            Condition = Conditions[Random.Shared.Next(Conditions.Length)],
            Humidity = Random.Shared.Next(30, 90) + "%",
            WindSpeed = Random.Shared.Next(5, 25) + " km/h",
            LastUpdated = DateTime.UtcNow.ToString("u")
        };

        return Task.FromResult(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }

    [McpServerTool]
    [Description("Gets a 5-day weather forecast for a specified city starting from tomorrow.")]
    public Task<string> GetWeatherForecast(
        [Description("Name of the city to get forecast for")] string city)
    {
        var payload = new
        {
            City = city,
            Forecast = Enumerable.Range(1, 5).Select(day => new
            {
                Date = DateTime.UtcNow.AddDays(day).ToString("yyyy-MM-dd"),
                HighTemp = Random.Shared.Next(15, 35) + "°C",
                LowTemp = Random.Shared.Next(-5, 20) + "°C",
                Condition = Conditions[Random.Shared.Next(Conditions.Length)],
                ChanceOfRain = Random.Shared.Next(0, 100) + "%"
            }).ToArray()
        };

        return Task.FromResult(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }
}
