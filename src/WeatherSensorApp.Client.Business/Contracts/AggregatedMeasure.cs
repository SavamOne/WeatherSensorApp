using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Business.Extensions;

namespace WeatherSensorApp.Client.Business.Contracts;

public class AggregatedMeasure
{
	private int humiditySum;
	private int measureCount;
	private decimal temperatureSum;

	public AggregatedMeasure(Guid sensorId, DateTime aggregatedMinute)
	{
		SensorId = sensorId;
		AggregatedMinute = aggregatedMinute.StripSeconds();
	}

	public AggregatedMeasure(Guid sensorId, DateTime aggregatedMinute, Measure firstMeasure)
		: this(sensorId, aggregatedMinute)
	{
		AppendMeasure(firstMeasure);
	}

	public Guid SensorId { get; }

	public DateTime AggregatedMinute { get; }

	public decimal MeanTemperature => temperatureSum / measureCount;

	public int MeanHumidity => humiditySum / measureCount;

	public int MinCo2 { get; protected set; }

	public int MaxCo2 { get; protected set; }

	public void AppendMeasure(Measure measure)
	{
		temperatureSum += measure.Temperature;
		humiditySum += measure.Humidity;
		MinCo2 = measureCount > 0 ? Math.Min(measure.Co2, MinCo2) : measure.Co2;
		MaxCo2 = Math.Max(measure.Co2, MaxCo2);

		measureCount++;
	}
}