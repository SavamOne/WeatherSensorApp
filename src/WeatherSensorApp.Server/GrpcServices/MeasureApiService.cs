using System.Threading.Channels;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Services;
using WeatherSensorApp.Server.Converters;

namespace WeatherSensorApp.Server.GrpcServices;

public class MeasureApiService : MeasureSubscriptionService.MeasureSubscriptionServiceBase
{
	private readonly Channel<MeasureResponse> measureChannel = Channel.CreateUnbounded<MeasureResponse>();
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
		Task readTask = Task.Run(async () =>
		{
			Dictionary<Guid, Guid> sensorSubscriptionIds = new();
			try
			{
				await foreach (MeasureRequest request in requestStream.ReadAllAsync(context.CancellationToken))
				{
					ProcessRequest(request, sensorSubscriptionIds, context.CancellationToken);
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
		});
		
		Task writeTask = Task.Run(async () =>
		{
			await foreach (MeasureResponse response in measureChannel.Reader.ReadAllAsync(context.CancellationToken))
			{
				await responseStream.WriteAsync(response, context.CancellationToken);
			}
		});

		await Task.WhenAny(readTask, writeTask);
	}
	
	private  async Task OnNewMeasure(Measure measure, CancellationToken cancellationToken)
	{
		await measureChannel.Writer.WriteAsync(measure.ConvertToGrpcPresentation(), cancellationToken);
	}

	private void ProcessRequest(MeasureRequest request,
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
			sensorSubscriptionIds[sensorId] = service.SubscribeToMeasures(sensorId, async measure => await OnNewMeasure(measure, cancellationToken), cancellationToken);
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