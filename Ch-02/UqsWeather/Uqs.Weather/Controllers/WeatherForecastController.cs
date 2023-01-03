using AdamTibi.OpenWeather;
using Microsoft.AspNetCore.Mvc;

namespace Uqs.Weather.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	private static readonly string[] Summaries = new[]
	{
		"Feezing", "Bracing", "Chilly", "Cool", "Mild",
		"Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	private readonly ILogger<WeatherForecastController> _logger;

	public WeatherForecastController(ILogger<WeatherForecastController> logger)
	{
		_logger = logger;
	}

	private string MapFeelToTemp(int temperatureC)
	{
		if (temperatureC <= 0) return Summaries.First();
		int summariesIndex = (temperatureC / 5) + 1;
		if (summariesIndex >= Summaries.Length) return Summaries.Last();
		return Summaries[summariesIndex];
	}

	[HttpGet("GetRandomWeatherForecast")]
	public IEnumerable<WeatherForecast> GetRandom()
	{
		WeatherForecast[] wfs = new WeatherForecast[5];
		for (int i = 0; i < wfs.Length; i++)
		{
			var wf = wfs[i] = new WeatherForecast();
			wf.Date = DateTime.Now.AddDays(i + 1);
			wf.TemperatureC = Random.Shared.Next(-20, 55);
			wf.Summary = MapFeelToTemp(wf.TemperatureC);
		}
		return wfs;
	}

	[HttpGet("GetRealWeatherForecast")]
	public async Task<IEnumerable<WeatherForecast>> GetReal()
	{
		string apiKey = _config["OpenWeather:Key"];
		HttpClient httpClient = new HttpClient();
		Client openWeatherClient = new Client(apiKey, httpClient);
		OneCallResponse res = await openWeatherClient.OneCallAsync(
			GREENWICH_LAT, GREENWICH_LON, new[]
			{
				Excludes.Current, Excludes.Minutely,
				Excludes.Hourly, Excludes.Alerts
			},
			Units.Metric);
	}
}
