using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Business.Contracts;

namespace WeatherSensorApp.Client.Business.Services;

public interface IAggregatedMeasureService
{
	public event Action<SensorSubscriptionEventArgs>? SensorsCollectionChanged;
	
	void SubscribeSensor(Guid sensorId);

	void UnsubscribeSensor(Guid sensorId);

	void AppendMeasure(Measure measure);

	TotalAggregatedMeasure GetRange(Guid sensorId, DateTime startTime, int minutes);

	IReadOnlyCollection<AggregatedMeasure> GetAll();
}