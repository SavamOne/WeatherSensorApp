using System.ComponentModel.DataAnnotations;

namespace WeatherSensorApp.Server.Business.Options;

public class SensorDefinition
{
	// Добавлено для упрощения тестирования
	public Guid? Id { get; set; }

	[MinLength(1)]
	public string Name { get; set; } = null!;
	
	public SensorDefinitionType Type { get; set; }
}