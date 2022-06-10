using System.ComponentModel.DataAnnotations;

namespace WeatherSensorApp.Server.Options;

public class MeasureOptions
{
	[Range(100, 2000)]
	public int IntervalInMillis { get; set; }
}