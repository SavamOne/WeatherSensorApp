namespace WeatherSensorApp.Client.Contracts;

public class TotalAggregatedMeasureResult
{
	public Guid SensorId { get; init; }

	public DateTime AggregatedMinuteStart { get; init; }

	public DateTime AggregatedMinuteEnd { get; init; }
	
	public decimal MeanTemperature {get; init; }

	public int MeanHumidity { get; init; }
	
	public int MinCo2 { get; init; }
	
	public int MaxCo2 { get; init; }
}