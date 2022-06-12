using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Contracts;

namespace WeatherSensorApp.Client.Converters;

public static class TotalAggregatedMeasureConverter
{
	public static TotalAggregatedMeasureResult ConvertToPresentation(this TotalAggregatedMeasure totalAggregatedMeasure)
	{
		return new TotalAggregatedMeasureResult
		{
			AggregatedMinuteStart = totalAggregatedMeasure.AggregatedTimeStart,
			AggregatedMinuteEnd = totalAggregatedMeasure.AggregatedTimeEnd,
			MaxCo2 = totalAggregatedMeasure.MaxCo2,
			MeanHumidity = totalAggregatedMeasure.MeanHumidity,
			MeanTemperature = Math.Round(totalAggregatedMeasure.MeanTemperature, 1),
			MinCo2 = totalAggregatedMeasure.MinCo2,
			SensorId = totalAggregatedMeasure.SensorId
		};
	}
}