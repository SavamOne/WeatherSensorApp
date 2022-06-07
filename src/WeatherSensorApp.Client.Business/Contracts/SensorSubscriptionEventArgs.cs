namespace WeatherSensorApp.Client.Business.Contracts;

public class SensorSubscriptionEventArgs : EventArgs
{
	public SensorSubscriptionEventArgs(Guid sensorId, bool subscribe)
	{
		SensorId = sensorId;
		Subscribe = subscribe;
	}

	public Guid SensorId { get; }
	
	public bool Subscribe { get; }
}