﻿using System.Threading.Channels;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Business.Services;
using WeatherSensorApp.Client.Converters;
using WeatherSensorApp.Server;

namespace WeatherSensorApp.Client.GrpcClientServices.Implementations;

public class MeasureApiClientService : IMeasureApiClientService
{
	private readonly MeasureSubscriptionService.MeasureSubscriptionServiceClient client;
	private readonly IAggregatedMeasureService measureService;
	private readonly Channel<MeasureRequest> requestsChannel = Channel.CreateUnbounded<MeasureRequest>();
	
	public MeasureApiClientService(MeasureSubscriptionService.MeasureSubscriptionServiceClient client, IAggregatedMeasureService measureService)
	{
		this.client = client;
		this.measureService = measureService;
		this.measureService.SensorsCollectionChanged += MeasureServiceOnSensorsCollectionChanged;
	}
	
	private async void MeasureServiceOnSensorsCollectionChanged(SensorSubscriptionEventArgs eventArgs)
	{
		await requestsChannel.Writer.WriteAsync(new MeasureRequest
		{
			SensorId = eventArgs.SensorId.ToString(),
			Subscribe = eventArgs.Subscribe
		});
	}
	
	public async Task RunMeasureStreamAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await ProcessBidirectionalCall(stoppingToken);
			}
			catch (Exception e)
			{
				//logger.LogError(e, "Unknown exception while processing grpc call");
				await Task.Delay(1000, stoppingToken);
			}
		}
	}
	
	public async Task<IReadOnlyCollection<Sensor>> GetAvailableSensorsAsync()
	{
		AvailableSensorsResponse result = await client.GetAvailableSensorsAsync(new Empty());

		return result.Sensors.Select(SensorConverter.ConvertToBusiness).ToList();
	}

	private async Task ProcessBidirectionalCall(CancellationToken stoppingToken)
	{
		AsyncDuplexStreamingCall<MeasureRequest, MeasureResponse>? call = client.StreamMeasures(cancellationToken: stoppingToken);

		Task readTask = Task.Run(async () =>
		{
			await foreach (MeasureResponse response in call.ResponseStream.ReadAllAsync(stoppingToken))
			{
				measureService.AppendMeasure(MeasureConverter.ConvertToBusiness(response));
			}
		}, stoppingToken);
		
		Task writeTask = Task.Run(async () =>
		{
			await foreach (MeasureRequest request in requestsChannel.Reader.ReadAllAsync(stoppingToken))
			{
				await call.RequestStream.WriteAsync(request, stoppingToken);
			}
		}, stoppingToken);

		await Task.WhenAny(readTask, writeTask);
	}
}