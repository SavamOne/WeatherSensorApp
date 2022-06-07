namespace WeatherSensorApp.Client.Contracts;

public class SensorResult
{
	public Guid Id { get; init; }

	public string Type { get; init; }

	public string Name { get; init; }
}