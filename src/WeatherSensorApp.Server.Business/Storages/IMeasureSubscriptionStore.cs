using WeatherSensorApp.Business.Contracts;

namespace WeatherSensorApp.Server.Business.Storages;

public interface IMeasureSubscriptionStore
{
	IReadOnlyCollection<MeasureSubscription> GetSubscriptions(Guid sensorId);

	void RemoveSubscription(Guid sensorId, Guid subscriptionId);

	void AddSubscription(MeasureSubscription subscription);
}