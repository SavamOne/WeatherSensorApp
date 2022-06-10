using System.ComponentModel.DataAnnotations;

namespace WeatherSensorApp.Server.Business.Options;

public class SensorDefinition
{
	// Добавлено для упрощения тестирования
	public Guid? Id { get; set; }
	
	[Required]
	public string Name { get; set; }
	
	public SensorDefinitionType Type { get; set; }
}