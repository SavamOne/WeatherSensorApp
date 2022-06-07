using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Contracts;
using WeatherSensorApp.Server;

namespace WeatherSensorApp.Client.Converters;

public static class SensorConverter
{
	public static Sensor ConvertToBusiness(this SensorResponse sensorResponse)
	{
		WeatherSensorApp.Business.Contracts.SensorType sensorType = sensorResponse.SensorType switch
		{
			Server.SensorType.Indoor => WeatherSensorApp.Business.Contracts.SensorType.Indoor,
			Server.SensorType.Outdoor => WeatherSensorApp.Business.Contracts.SensorType.Outdoor,
			_ => throw new ArgumentOutOfRangeException(nameof(sensorResponse.SensorType))
		};

		if (!Guid.TryParse(sensorResponse.Id, out Guid id))
		{
			throw new ArgumentException(nameof(sensorResponse.Id));
		}

		return new Sensor(id, sensorType, sensorResponse.Name);
	}

	public static SensorResult ConvertToPresentation(this Sensor sensor)
	{
		return new SensorResult
		{
			Id = sensor.Id,
			Name = sensor.Name,
			Type = sensor.Type.ToString("G")
		};
	}
}