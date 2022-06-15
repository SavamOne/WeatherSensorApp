using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Converters;
using WeatherSensorApp.Server.Business.Options;
using WeatherSensorApp.Server.Business.Storages;

namespace WeatherSensorApp.Server.Business.Services.Implementations;

public class MeasureService : IMeasureService
{
	private readonly ILastMeasureStore lastMeasureStore;
	private readonly ILogger<MeasureService> logger;
	private readonly IMeasureSubscriptionStore subscriptionStore;
	private readonly Dictionary<Guid, Sensor> sensors;


	public MeasureService(ILastMeasureStore lastMeasureStore, IMeasureSubscriptionStore subscriptionStore, IOptions<SensorOptions> options, ILogger<MeasureService> logger)
	{
		sensors = options.Value.SensorDefinitions.Select(SensorConverter.ConvertToBusiness).ToDictionary(x => x.Id);
		
		this.lastMeasureStore = lastMeasureStore;
		this.subscriptionStore = subscriptionStore;
		this.logger = logger;
	}

	public void OnNewMeasure(Measure measure, CancellationToken stoppingToken)
	{
		if (!sensors.ContainsKey(measure.SensorId))
		{
			throw new ArgumentException(nameof(measure.SensorId));
		}

		lastMeasureStore.UpdateLastMeasure(measure);
		
		foreach (MeasureSubscription subscription in subscriptionStore.GetSubscriptions(measure.SensorId))
		{
			_ = Task.Run(async () =>
			{
				using CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(subscription.CancellationToken, stoppingToken);
				
				if (source.IsCancellationRequested)
				{
					logger.LogInformation("Removed subscription");
					subscriptionStore.RemoveSubscription(subscription.SensorId, subscription.Id);
					return;
				}
				
				await subscription.Callback(measure, source.Token);
			}, CancellationToken.None);
		}
	}
	
	public Guid SubscribeToMeasures(Guid sensorId, Func<Measure, CancellationToken, Task> callback, CancellationToken cancellationToken)
	{
		if (!sensors.ContainsKey(sensorId))
		{
			throw new ArgumentException(nameof(sensorId));
		}

		MeasureSubscription subscription = new(Guid.NewGuid(), sensorId, cancellationToken, callback);
		subscriptionStore.AddSubscription(subscription);
		return subscription.Id;
	}

	public void UnsubscribeFromMeasures(Guid sensorId, Guid subscriptionId)
	{
		subscriptionStore.RemoveSubscription(sensorId, subscriptionId);
	}

	public IReadOnlyCollection<Sensor> GetAvailableSensors()
	{
		return sensors.Values;
	}

	public Measure? GetLastMeasure(Guid sensorId)
	{
		if (!sensors.ContainsKey(sensorId))
		{
			throw new ArgumentException(nameof(sensorId));
		}

		return lastMeasureStore.GetLastMeasure(sensorId);
	}
}