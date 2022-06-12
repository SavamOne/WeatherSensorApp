using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Contracts;

namespace WeatherSensorApp.Client.Converters;

public static class AggregatedMeasureConverter
{
	public static AggregatedMeasureResult ConvertToPresentation(this AggregatedMeasure aggregatedMeasure)
	{
		return new AggregatedMeasureResult
		{
			AggregatedStartTime = aggregatedMeasure.AggregatedTimeStart,
			AggregatedEndTime = aggregatedMeasure.AggregatedTimeEnd,
			MaxCo2 = aggregatedMeasure.MaxCo2,
			MeanHumidity = aggregatedMeasure.MeanHumidity,
			MeanTemperature = Math.Round(aggregatedMeasure.MeanTemperature, 1),
			MinCo2 = aggregatedMeasure.MinCo2,
			SensorId = aggregatedMeasure.SensorId
		};
	}
}