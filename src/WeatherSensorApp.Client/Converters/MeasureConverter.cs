using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server;

namespace WeatherSensorApp.Client.Converters;

public static class MeasureConverter
{
	public static Measure ConvertToBusiness(this MeasureResponse measureResponse)
	{
		if (!Guid.TryParse(measureResponse.SensorId, out Guid sensorId))
		{
			throw new ArgumentException(nameof(measureResponse.SensorId));
		}

		return new Measure(sensorId,
			measureResponse.Time.ToDateTime(),
			Convert.ToDecimal(measureResponse.Temperature),
			measureResponse.Humidity,
			measureResponse.Co2);
	}
}