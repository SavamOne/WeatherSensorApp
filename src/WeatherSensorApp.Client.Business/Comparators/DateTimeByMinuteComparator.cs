namespace WeatherSensorApp.Client.Business.Comparators;

public class DateTimeByMinuteComparator : IEqualityComparer<DateTime>
{
	public bool Equals(DateTime x, DateTime y)
	{
		return x.Day == y.Day
			&& x.Hour == y.Hour
			&& x.Minute == y.Minute
			&& x.Month == y.Month
			&& x.Year == y.Year;
	}

	public int GetHashCode(DateTime obj)
	{
		return HashCode.Combine(obj.Day,
			obj.Hour,
			obj.Minute,
			obj.Month,
			obj.Year);
	}
}