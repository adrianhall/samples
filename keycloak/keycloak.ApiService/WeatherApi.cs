public static class WeatherApi
{
    public static IEnumerable<WeatherForecast> GetWeather()
    {
        var rng = new Random();
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            rng.Next(-20, 55),
            summaries[rng.Next(summaries.Length)]
        )).ToArray();

        return forecast;
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}