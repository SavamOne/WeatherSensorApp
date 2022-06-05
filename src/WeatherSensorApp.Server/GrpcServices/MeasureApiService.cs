using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Services;

namespace WeatherSensorApp.Server.GrpcServices;

public class MeasureApiService : MeasureSubscriptionService.MeasureSubscriptionServiceBase
{
	private readonly ILogger<MeasureApiService> logger;
	private readonly IMeasureService service;

	public MeasureApiService(IMeasureService service, ILogger<MeasureApiService> logger)
	{
		this.service = service;
		this.logger = logger;
	}

	public override Task<AvailableSensorsResponse> GetAvailableSensors(Empty request, ServerCallContext context)
	{
		AvailableSensorsResponse response = new()
		{
			Sensors = { service.GetAvailableSensors().Select(ConvertToGrpcType) }
		};

		return Task.FromResult(response);
	}

	public override async Task StreamMeasures(IAsyncStreamReader<MeasureRequest> requestStream, IServerStreamWriter<MeasureResponse> responseStream, ServerCallContext context)
	{
		Dictionary<Guid, Guid> sensorSubscriptionIds = new();

		try
		{
			await foreach (MeasureRequest request in requestStream.ReadAllAsync(context.CancellationToken))
			{
				ProcessRequest(request, responseStream, sensorSubscriptionIds, context.CancellationToken);
			}
		}
		finally
		{
			foreach (var kvp in sensorSubscriptionIds)
			{
				service.UnsubscribeFromMeasures(kvp.Key, kvp.Value);
				logger.LogDebug("Force unsubscribed!");
			}
		}
	}
	
	private static SensorResponse ConvertToGrpcType(Sensor sensor)
	{
		return new SensorResponse
		{
			Id = sensor.Id.ToString(),
			Name = sensor.Name,
			SensorType = sensor.Type switch
			{
				WeatherSensorApp.Business.Contracts.SensorType.Indoor => SensorType.Indoor,
				WeatherSensorApp.Business.Contracts.SensorType.Outdoor => SensorType.Outdoor,
				_ => throw new ArgumentOutOfRangeException(nameof(sensor.Type))
			}
		};
	}
	
	private static async Task OnNewMeasure(IAsyncStreamWriter<MeasureResponse> responseStream, Measure measure)
	{
		MeasureResponse response = new()
		{
			SensorId = measure.SensorId.ToString(),
			Co2 = measure.Co2,
			Humidity = measure.Humidity,
			Temperature = Convert.ToDouble(measure.Temperature),
			Time = Timestamp.FromDateTime(measure.MeasureTime)
		};

		await responseStream.WriteAsync(response);
	}
	
	private void ProcessRequest(MeasureRequest request, IAsyncStreamWriter<MeasureResponse> responseStream, Dictionary<Guid, Guid> sensorSubscriptionIds, CancellationToken cancellationToken)
	{
		if (!Guid.TryParse(request.SensorId, out Guid sensorId))
		{
			return;
		}

		bool containsSub = sensorSubscriptionIds.TryGetValue(sensorId, out Guid subscriptionId);

		if (request.Subscribe && !containsSub)
		{
			sensorSubscriptionIds[sensorId] = service.SubscribeToMeasures(sensorId, async measure => await OnNewMeasure(responseStream, measure), cancellationToken);
			logger.LogDebug("Subscribed!");
		}
		else if (!request.Subscribe && containsSub)
		{
			service.UnsubscribeFromMeasures(sensorId, subscriptionId);
			sensorSubscriptionIds.Remove(sensorId);
			logger.LogDebug("Unsubscribed!");
		}
	}
}