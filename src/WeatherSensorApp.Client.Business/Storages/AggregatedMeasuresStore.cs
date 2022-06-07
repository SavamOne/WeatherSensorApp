using System.Collections.Concurrent;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Business.Comparators;
using WeatherSensorApp.Client.Business.Contracts;

namespace WeatherSensorApp.Client.Business.Storages;

public class AggregatedMeasuresStore
{
	private static readonly IEqualityComparer<DateTime> DateTimeComparator = new DateTimeByMinuteComparator();

	private readonly ConcurrentDictionary<DateTime, AggregatedMeasure> aggregatedMeasures = new(DateTimeComparator);

	public void AppendMeasure(Measure measure)
	{
		aggregatedMeasures.AddOrUpdate(measure.MeasureTime,
		_ => new AggregatedMeasure(measure.SensorId, measure.MeasureTime, measure),
		(_, aggregatedMeasure) =>
		{
			aggregatedMeasure.AppendMeasure(measure);
			return aggregatedMeasure;
		});
	}

	public AggregatedMeasure? GetByMinute(DateTime dateTime)
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
		   .OrderBy(x => x.AggregatedMinute)
		   .ToList();
	}
}