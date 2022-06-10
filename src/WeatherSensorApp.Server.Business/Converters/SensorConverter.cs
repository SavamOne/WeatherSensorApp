using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Options;

namespace WeatherSensorApp.Server.Business.Converters;

public static class SensorConverter
{
	public static Sensor ConvertToBusiness(this SensorDefinition sensorDefinition)
	{
		return new Sensor(sensorDefinition.Id ?? Guid.NewGuid(), sensorDefinition.Type.ConvertToBusiness(), sensorDefinition.Name);
	}

	public static SensorType ConvertToBusiness(this SensorDefinitionType sensorDefinitionType)
	{
		return sensorDefinitionType switch
		{
			SensorDefinitionType.Indoor => SensorType.Indoor,
			SensorDefinitionType.Outdoor => SensorType.Outdoor,
			_ => throw new ArgumentOutOfRangeException(nameof(sensorDefinitionType), sensorDefinitionType, null)
		};
	}
}