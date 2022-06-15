using WeatherSensorApp.Business.Contracts;

namespace WeatherSensorApp.Server.Business.Services;

public interface IMeasureService
{
	void OnNewMeasure(Measure measure, CancellationToken cancellationToken);

	Guid SubscribeToMeasures(Guid sensorId, Func<Measure, CancellationToken, Task> callback, CancellationToken cancellationToken);

	void UnsubscribeFromMeasures(Guid sensorId, Guid subscriptionId);

	IReadOnlyCollection<Sensor> GetAvailableSensors();

	Measure? GetLastMeasure(Guid sensorId);
}