namespace WeatherSensorApp.Client.Contracts;

public class AggregatedMeasureResult
{
	public Guid SensorId { get; init; }

	public DateTime AggregatedStartTime { get; init; }
	
	public DateTime AggregatedEndTime { get; init; }

	public decimal MeanTemperature { get; init; }

	public int MeanHumidity { get; init; }

	public int MinCo2 { get; init; }

	public int MaxCo2 { get; init; }
}