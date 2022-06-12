using System.Collections.Concurrent;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Business.Helpers;

namespace WeatherSensorApp.Client.Business.Storages;

public class AggregatedMeasuresStore
{
	private readonly IAggregationHelper aggregationHelper;
	private readonly ConcurrentDictionary<DateTime, AggregatedMeasure> aggregatedMeasures;

	public AggregatedMeasuresStore(IAggregationHelper aggregationHelper)
	{
		this.aggregationHelper = aggregationHelper;
		aggregatedMeasures = new ConcurrentDictionary<DateTime, AggregatedMeasure>(aggregationHelper.DateTimeComparer);
	}

	public void AppendMeasure(Measure measure)
	{
		aggregatedMeasures.AddOrUpdate(measure.MeasureTime,
		_ =>
		{
			AggregatedMeasure store = new(measure.SensorId,
				aggregationHelper.RoundToAggregationPeriodStart(measure.MeasureTime),
				aggregationHelper.RoundToAggregationEnd(measure.MeasureTime));
			store.AppendMeasure(measure);
			return store;
		},
		(_, aggregatedMeasure) =>
		{
			aggregatedMeasure.AppendMeasure(measure);
			return aggregatedMeasure;
		});
	}

	public AggregatedMeasure? GetByPediod(DateTime dateTime)
	{
		if (aggregatedMeasures.TryGetValue(dateTime, out AggregatedMeasure? measure))
		{
			return measure;
		}

		return null;
	}

	public IReadOnlyCollection<AggregatedMeasure> GetAll()
	{
		return aggregatedMeasures.Values
		   .OrderBy(x => x.AggregatedTimeStart)
		   .ToList();
	}
}