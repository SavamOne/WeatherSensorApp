using Microsoft.Extensions.Options;
using WeatherSensorApp.Client.Business.Options;

namespace WeatherSensorApp.Client.Business.Helpers.Implementations;

public class ByMinuteAggregationHelper : IAggregationHelper
{
	private readonly int aggregationMinutes;
	
	public ByMinuteAggregationHelper(IOptions<AggregationOptions> options)
	{
		aggregationMinutes = options.Value.AggregationPeriodInMinutes;
		
		if (aggregationMinutes <= 0 || 60 % aggregationMinutes != 0)
		{
			throw new ArgumentException(nameof(aggregationMinutes));
		}
		
		DateTimeComparer = new DateTimeByMinuteComparator(aggregationMinutes);
		AggregationTime = TimeSpan.FromMinutes(aggregationMinutes);
	}

	public IEqualityComparer<DateTime> DateTimeComparer { get; }

	public TimeSpan AggregationTime { get; }

	public DateTime RoundToAggregationPeriodStart(DateTime dateTime)
	{
		return new DateTime(dateTime.Year,
			dateTime.Month,
			dateTime.Day,
			dateTime.Hour,
			dateTime.Minute - dateTime.Minute % aggregationMinutes,
			0,
			dateTime.Kind);
	}
	
	public DateTime RoundToAggregationEnd(DateTime dateTime)
	{
		return RoundToAggregationPeriodStart(dateTime).AddMinutes(aggregationMinutes).AddMilliseconds(-1);
	}
	
	public DateTime RoundToAggregationEnd(DateTime dateTime, int periods)
	{
		if (periods < 0)
		{
			throw new ArgumentException(nameof(periods));
		}
		
		return RoundToAggregationPeriodStart(dateTime).AddMinutes(aggregationMinutes * periods).AddMilliseconds(-1);
	}

	private class DateTimeByMinuteComparator : IEqualityComparer<DateTime>
	{
		private readonly int aggregationMinutes;
	
		public DateTimeByMinuteComparator(int aggregationMinutes)
		{
			this.aggregationMinutes = aggregationMinutes;
		}
	
		public bool Equals(DateTime x, DateTime y)
		{
			return x.Day == y.Day
				&& x.Hour == y.Hour
				&& x.Minute / aggregationMinutes == y.Minute / aggregationMinutes
				&& x.Month == y.Month
				&& x.Year == y.Year;
		}

		public int GetHashCode(DateTime obj)
		{
			return HashCode.Combine(obj.Day,
				obj.Hour,
				obj.Minute / aggregationMinutes,
				obj.Month,
				obj.Year);
		}
	}
}