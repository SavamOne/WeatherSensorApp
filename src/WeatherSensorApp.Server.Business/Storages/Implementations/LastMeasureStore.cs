using System.Collections.Concurrent;
using WeatherSensorApp.Business.Contracts;

namespace WeatherSensorApp.Server.Business.Storages.Implementations;

public class LastMeasureStore : ILastMeasureStore
{
	private readonly ConcurrentDictionary<Guid, Measure> measures = new();

	public void UpdateLastMeasure(Measure measure)
	{
		measures.AddOrUpdate(measure.SensorId, measure, (_, _) => measure);
	}

	public Measure? GetLastMeasure(Guid sensorId)
	{
		if (measures.TryGetValue(sensorId, out Measure? measure))
		{
			return measure;
		}

		return null;
	}
}