using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Services;
using WeatherSensorApp.Server.Converters;

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
			Sensors =
			{
				service.GetAvailableSensors().Select(SensorConverter.ConvertToGrpcPresentation)
			}
		};

		return Task.FromResult(response);
	}

	public override async Task StreamMeasures(IAsyncStreamReader<MeasureRequest> requestStream, IServerStreamWriter<MeasureResponse> responseStream, ServerCallContext context)
	{
		Dictionary<Guid, Guid> sensorSubscriptionIds = new();

		try
		{
			// TODO почему не разделил на два параллельных метода один из которых отвечает за прием второй за передачу
			// TODO так было бы проще контролировать, такой пример даже вроде на майкрософте есть
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

	// TODO не передаешь токен отмены
	private static async Task OnNewMeasure(IAsyncStreamWriter<MeasureResponse> responseStream, Measure measure)
	{
		await responseStream.WriteAsync(measure.ConvertToGrpcPresentation());
	}

	private void ProcessRequest(MeasureRequest request,
		IAsyncStreamWriter<MeasureResponse> responseStream,
		Dictionary<Guid, Guid> sensorSubscriptionIds,
		CancellationToken cancellationToken)
	{
		if (!Guid.TryParse(request.SensorId, out Guid sensorId))
		{
			return;
		}

		bool containsSub = sensorSubscriptionIds.TryGetValue(sensorId, out Guid subscriptionId);

		if (request.Subscribe && !containsSub)
		{
			// TODO Соответсвенно тут надо тоже передать токен отмены
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