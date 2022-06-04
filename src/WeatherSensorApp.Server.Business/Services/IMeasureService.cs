using WeatherSensorApp.Server.Business.Contracts;

namespace WeatherSensorApp.Server.Business.Services;

public interface IMeasureService
{
	void OnNewMeasure(Measure measure);

	Guid SubscribeToMeasures(Guid sensorId, Func<Measure, Task> callback, CancellationToken cancellationToken);

	void UnsubscribeFromMeasures(Guid sensorId, Guid subscriptionId);

	IReadOnlyCollection<Sensor> GetAvailableSensors();

	Measure? GetLastMeasure(Guid sensorId);
}