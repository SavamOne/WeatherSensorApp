namespace WeatherSensorApp.Server.Business.Contracts;

public class Measure
{
	public Measure(Guid sensorId,
		DateTime measureTime,
		decimal temperature,
		int humidity,
		int co2)
	{
		Temperature = temperature;
		Humidity = humidity;
		Co2 = co2;
		SensorId = sensorId;
		MeasureTime = measureTime;
	}

	public Guid SensorId { get; }

	public DateTime MeasureTime { get; }

	public decimal Temperature { get; }

	public int Humidity { get; }

	public int Co2 { get; }
}