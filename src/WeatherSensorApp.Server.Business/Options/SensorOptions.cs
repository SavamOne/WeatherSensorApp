using System.ComponentModel.DataAnnotations;

namespace WeatherSensorApp.Server.Business.Options;

public class SensorOptions
{
	[Required]
	public List<SensorDefinition> SensorDefinitions { get; set; } = null!;
}