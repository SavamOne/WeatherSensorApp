using Google.Protobuf.WellKnownTypes;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Contracts;

namespace WeatherSensorApp.Server.Converters;

public static class MeasureConverter
{
	public static MeasureResult ConvertToPresentation(this Measure measure)
	{
		return new MeasureResult
		{
			Co2 = measure.Co2,
			Humidity = measure.Humidity,
			Temperature = measure.Temperature,
			MeasureTime = measure.MeasureTime,
			SensorId = measure.SensorId
		};
	}

	public static MeasureResponse ConvertToGrpcPresentation(this Measure measure)
	{
		return new MeasureResponse
		{
			SensorId = measure.SensorId.ToString(),
			Co2 = measure.Co2,
			Humidity = measure.Humidity,
			Temperature = Convert.ToDouble(measure.Temperature),
			Time = Timestamp.FromDateTime(measure.MeasureTime)
		};
	}
}