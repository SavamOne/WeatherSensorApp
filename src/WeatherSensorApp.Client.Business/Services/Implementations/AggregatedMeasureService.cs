using System.Collections.Concurrent;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Business.Helpers;
using WeatherSensorApp.Client.Business.Storages;

namespace WeatherSensorApp.Client.Business.Services.Implementations;

public class AggregatedMeasureService : IAggregatedMeasureService
{
	private readonly IAggregationHelper aggregationHelper;
	private readonly ConcurrentDictionary<Guid, AggregatedMeasuresStore> sensorDict = new();

	public AggregatedMeasureService(IAggregationHelper aggregationHelper) 
	{
		this.aggregationHelper = aggregationHelper;
	}

	public event Action<SensorSubscriptionEventArgs>? SensorsCollectionChanged;

	public void SubscribeSensor(Guid sensorId)
	{
		sensorDict.AddOrUpdate(sensorId, _ =>
		{
			AggregatedMeasuresStore store = new(aggregationHelper);
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

	public TotalAggregatedMeasure GetRange(Guid sensorId, DateTime startTime, int periods)
	{
		if (periods < 1)
		{
			throw new ArgumentException(nameof(periods));
		}

		if (!sensorDict.TryGetValue(sensorId, out AggregatedMeasuresStore? store))
		{
			throw new ArgumentException(nameof(sensorId));
		}

		DateTime aggregationStartTime = aggregationHelper.RoundToAggregationPeriodStart(startTime);
		DateTime aggregationEndTime = aggregationHelper.RoundToAggregationEnd(startTime, periods);
		
		TotalAggregatedMeasure totalAggregatedMeasure = new(sensorId, aggregationStartTime, aggregationEndTime);

		for (DateTime current = aggregationStartTime; current <= aggregationEndTime; current += aggregationHelper.AggregationTime)
		{
			AggregatedMeasure? aggregatedMeasure = store.GetByPediod(current);

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
		   .ThenBy(x => x.AggregatedTimeStart)
		   .ToList();
	}
}