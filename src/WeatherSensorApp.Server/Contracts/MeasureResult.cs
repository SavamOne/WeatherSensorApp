namespace WeatherSensorApp.Server.Contracts;

public class MeasureResult
{
	public Guid SensorId { get; init; }

	public DateTime MeasureTime { get; init; }

	public decimal Temperature { get; init; }

	public int Humidity { get; init; }

	public int Co2 { get; init; }
}