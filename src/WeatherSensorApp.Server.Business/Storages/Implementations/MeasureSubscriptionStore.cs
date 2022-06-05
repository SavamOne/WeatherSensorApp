using System.Collections.Concurrent;
using WeatherSensorApp.Business.Contracts;

namespace WeatherSensorApp.Server.Business.Storages.Implementations;

public class MeasureSubscriptionStore : IMeasureSubscriptionStore
{
	private readonly object locker = new();
	private readonly ConcurrentDictionary<Guid, HashSet<MeasureSubscription>> subscriptionsDict = new();

	public IReadOnlyCollection<MeasureSubscription> GetSubscriptions(Guid sensorId)
	{
		if (!subscriptionsDict.TryGetValue(sensorId, out var sensorSubscription))
		{
			return Array.Empty<MeasureSubscription>();
		}

		MeasureSubscription[] items;
		lock (locker)
		{
			items = sensorSubscription.ToArray();
		}

		return items;
	}

	public void RemoveSubscription(Guid sensorId, Guid subscriptionId)
	{
		if (!subscriptionsDict.TryGetValue(sensorId, out var sensorSubscription))
		{
			return;
		}

		//HACK: Подписки равные, если у них одинаковый id и sensorId. Создаем фейковую подписку, чтобы удалить настоящую из set-а.
		MeasureSubscription sub = new(subscriptionId, sensorId, CancellationToken.None, null!);

		lock (locker)
		{
			sensorSubscription.Remove(sub);
		}
	}

	public void AddSubscription(MeasureSubscription subscription)
	{
		if (subscriptionsDict.TryGetValue(subscription.SensorId, out var sensorSubscription))
		{
			lock (locker)
			{
				sensorSubscription.Add(subscription);
				return;
			}
		}

		sensorSubscription = new HashSet<MeasureSubscription>
		{
			subscription
		};

		subscriptionsDict.AddOrUpdate(subscription.SensorId,
			sensorSubscription,
			(_, set) =>
			{
				set.Add(subscription);
				return set;
			});
	}
}