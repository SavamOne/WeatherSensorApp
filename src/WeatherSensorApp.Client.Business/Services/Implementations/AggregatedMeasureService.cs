using System.Collections.Concurrent;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Business.Extensions;
using WeatherSensorApp.Client.Business.Storages;

namespace WeatherSensorApp.Client.Business.Services.Implementations;

public class AggregatedMeasureService : IAggregatedMeasureService
{
	private readonly ConcurrentDictionary<Guid, AggregatedMeasuresStore> sensorDict = new();

	public event Action<SensorSubscriptionEventArgs>? SensorsCollectionChanged;

	public void SubscribeSensor(Guid sensorId)
	{
		sensorDict.AddOrUpdate(sensorId, _ =>
		{
			AggregatedMeasuresStore store = new();
			SensorsCollectionChanged?.Invoke(new SensorSubscriptionEventArgs(sensorId, true));
			return store;
		}, 
		(_, existedMeasure) => existedMeasure);
	}

	public void UnsubscribeSensor(Guid sensorId)
	{
		if (sensorDict.TryRemove(sensorId, out _))
		{
			SensorsCollectionChanged?.Invoke(new SensorSubscriptionEventArgs(sensorId, false));
		}
	}

	public void AppendMeasure(Measure measure)
	{
		if (!sensorDict.TryGetValue(measure.SensorId, out AggregatedMeasuresStore? store))
		{
			throw new ArgumentException(nameof(measure));
		}

		store.AppendMeasure(measure);
	}

	public TotalAggregatedMeasure GetRange(Guid sensorId, DateTime startTime, int minutes)
	{
		if (minutes < 0)
		{
			throw new ArgumentException(nameof(minutes));
		}

		if (!sensorDict.TryGetValue(sensorId, out AggregatedMeasuresStore? store))
		{
			throw new ArgumentException(nameof(sensorId));
		}

		startTime = startTime.StripSeconds();
		DateTime endTime = startTime.AddMinutes(minutes);

		TotalAggregatedMeasure totalAggregatedMeasure = new(sensorId, startTime, endTime);

		for (DateTime current = startTime; current <= endTime; current = current.AddMinutes(1))
		{
			AggregatedMeasure? aggregatedMeasure = store.GetByMinute(current);

			if (aggregatedMeasure is null)
			{
				throw new ArgumentException($"Store does not contains value for {current}");
			}

			totalAggregatedMeasure.AddAggregatedMeasure(aggregatedMeasure);
		}

		return totalAggregatedMeasure;
	}

	public IReadOnlyCollection<AggregatedMeasure> GetAll()
	{
		return sensorDict.Values
		   .SelectMany(x => x.GetAll())
		   .OrderBy(x => x.SensorId)
		   .ThenBy(x => x.AggregatedMinute)
		   .ToList();
	}
}