﻿using WeatherSensorApp.Server.Business.Contracts;
using WeatherSensorApp.Server.Business.Storages;

namespace WeatherSensorApp.Server.Business.Services.Implementations;

public class MeasureService : IMeasureService
{
	private readonly ILastMeasureStore lastMeasureStore;

	private readonly Dictionary<Guid, Sensor> sensors = new Sensor[]
	{
		new(Guid.Parse("9295d744-192f-42f4-86eb-082e5510846b"), SensorType.Outdoor, "Уличный датчик 1"),
		new(Guid.Parse("b4fcab16-c6a1-4f5b-9b54-b3be516c249a"), SensorType.Outdoor, "Уличный датчик 2"),
		new(Guid.Parse("58edced9-fdb7-49c4-be20-76316037ec20"), SensorType.Indoor, "Датчик в помещении")
	}.ToDictionary(x => x.Id);

	private readonly IMeasureSubscriptionStore subscriptionStore;

	public MeasureService(ILastMeasureStore lastMeasureStore, IMeasureSubscriptionStore subscriptionStore)
	{
		this.lastMeasureStore = lastMeasureStore;
		this.subscriptionStore = subscriptionStore;
	}

	public void OnNewMeasure(Measure measure)
	{
		if (!sensors.ContainsKey(measure.SensorId))
		{
			throw new ArgumentException(nameof(measure.SensorId));
		}

		lastMeasureStore.UpdateLastMeasure(measure);

		foreach (MeasureSubscription valueTuple in subscriptionStore.GetSubscriptions(measure.SensorId))
		{
			if (valueTuple.CancellationToken.IsCancellationRequested)
			{
				subscriptionStore.RemoveSubscription(valueTuple.SensorId, valueTuple.Id);
				continue;
			}

			Task.Run(() => valueTuple.Callback(measure));
		}
	}

	public Guid SubscribeToMeasures(Guid sensorId, Func<Measure, Task> callback, CancellationToken cancellationToken)
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