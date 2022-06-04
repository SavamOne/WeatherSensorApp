using WeatherSensorApp.Server.Business.Contracts;

namespace WeatherSensorApp.Server.Business.Storages;

public interface ILastMeasureStore
{
	void UpdateLastMeasure(Measure measure);

	Measure? GetLastMeasure(Guid sensorId);
}