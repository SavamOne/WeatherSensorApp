namespace WeatherSensorApp.Client.Business.Extensions;

public static class DateTimeExtensions
{
	public static DateTime StripSeconds(this DateTime dateTime)
	{
		return new DateTime(dateTime.Year,
			dateTime.Month,
			dateTime.Day,
			dateTime.Hour,
			dateTime.Minute,
			0,
			dateTime.Kind);
	}
}