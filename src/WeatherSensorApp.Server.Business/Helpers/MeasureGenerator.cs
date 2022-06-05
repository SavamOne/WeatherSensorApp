using WeatherSensorApp.Business.Contracts;

namespace WeatherSensorApp.Server.Business.Helpers;

public static class MeasureGenerator
{
	private const int MinTemperature = 20 * 100;
	private const int MaxTemperature = 25 * 100;

	private const int MinHumidity = 30;
	private const int MaxHumidity = 60;

	private const int MinCo2 = 800;
	private const int MaxCo2 = 1000;

	public static Measure GenerateRandom(Guid sensorId)
	{
		Random random = Random.Shared;

		return new Measure(sensorId,
			DateTime.UtcNow,
			random.Next(MinTemperature, MaxTemperature) / 100.0m,
			random.Next(MinHumidity, MaxHumidity),
			random.Next(MinCo2, MaxCo2));
	}
}