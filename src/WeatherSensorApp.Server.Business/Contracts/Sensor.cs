namespace WeatherSensorApp.Server.Business.Contracts;

public class Sensor
{
	public Sensor(Guid id, SensorType type, string name)
	{
		Id = id;
		Type = type;
		Name = name;
	}

	public Guid Id { get; }

	public SensorType Type { get; }

	public string Name { get; }
}