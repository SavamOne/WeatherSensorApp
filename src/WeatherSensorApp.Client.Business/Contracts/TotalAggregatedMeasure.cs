namespace WeatherSensorApp.Client.Business.Contracts;

public class TotalAggregatedMeasure
{
	private int humiditySum;
	private int measureCount;
	private decimal temperatureSum;

	public TotalAggregatedMeasure(Guid sensorId, DateTime aggregatedTimeStart, DateTime aggregatedTimeEnd)
	{
		SensorId = sensorId;
		AggregatedTimeStart = aggregatedTimeStart;
		AggregatedTimeEnd = aggregatedTimeEnd;
	}

	public Guid SensorId { get; }

	public DateTime AggregatedTimeStart { get; }

	public DateTime AggregatedTimeEnd { get; }

	public decimal MeanTemperature => temperatureSum / measureCount;

	public int MeanHumidity => humiditySum / measureCount;

	public int MinCo2 { get; private set; }

	public int MaxCo2 { get; private set; }

	public void AddAggregatedMeasure(AggregatedMeasure aggregatedMeasure)
	{
		temperatureSum += aggregatedMeasure.MeanTemperature;
		humiditySum += aggregatedMeasure.MeanHumidity;
		MinCo2 = measureCount > 0 ? Math.Min(aggregatedMeasure.MinCo2, MinCo2) : aggregatedMeasure.MinCo2;
		MaxCo2 = Math.Max(aggregatedMeasure.MaxCo2, MaxCo2);

		measureCount++;
	}
}