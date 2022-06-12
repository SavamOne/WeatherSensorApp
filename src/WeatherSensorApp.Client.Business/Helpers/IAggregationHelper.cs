namespace WeatherSensorApp.Client.Business.Helpers;

public interface IAggregationHelper
{
	IEqualityComparer<DateTime> DateTimeComparer { get; }
	
	TimeSpan AggregationTime { get; }

	DateTime RoundToAggregationPeriodStart(DateTime dateTime);

	DateTime RoundToAggregationEnd(DateTime dateTime);
	
	DateTime RoundToAggregationEnd(DateTime dateTime, int periods);
}