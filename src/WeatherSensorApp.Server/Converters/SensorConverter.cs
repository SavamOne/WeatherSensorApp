using WeatherSensorApp.Business.Contracts;

namespace WeatherSensorApp.Server.Converters;

public static class SensorConverter
{
	public static SensorResponse ConvertToGrpcPresentation(this Sensor sensor)
	{
		return new SensorResponse
		{
			Id = sensor.Id.ToString(),
			Name = sensor.Name,
			SensorType = sensor.Type switch
			{
				WeatherSensorApp.Business.Contracts.SensorType.Indoor => SensorType.Indoor,
				WeatherSensorApp.Business.Contracts.SensorType.Outdoor => SensorType.Outdoor,
				_ => throw new ArgumentOutOfRangeException(nameof(sensor.Type))
			}
		};
	} 
}