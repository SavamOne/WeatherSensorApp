using Grpc.Core;
using WeatherSensorApp.Server.Business.Contracts;
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

	public override async Task StreamMeasures(IAsyncStreamReader<MeasureRequest> requestStream, IServerStreamWriter<MeasureResponse> responseStream, ServerCallContext context)
	{
		Dictionary<Guid, Guid> subscriptionIds = new();

		while (!context.CancellationToken.IsCancellationRequested)
		{
			await foreach (MeasureRequest request in requestStream.ReadAllAsync())
			{
				if (!Guid.TryParse(request.SensorId, out Guid sensorId))
				{
					continue;
				}

				bool containsSub = subscriptionIds.TryGetValue(sensorId, out Guid subscriptionId);

				switch (request.Subscribe)
				{
					case true when !containsSub:
						subscriptionIds[sensorId] = service.SubscribeToMeasures(sensorId, async measure => await OnNewMeasure(responseStream, measure), context.CancellationToken);
						logger.LogInformation("Subscribed!");
						break;

					case false when containsSub:
						service.UnsubscribeFromMeasures(sensorId, subscriptionId);
						subscriptionIds.Remove(sensorId);
						logger.LogInformation("Unsubscribed!");
						break;
				}
			}
		}
	}

	private static async Task OnNewMeasure(IServerStreamWriter<MeasureResponse> responseStream, Measure measure)
	{
		MeasureResponse response = new()
		{
			SensorId = measure.SensorId.ToString(),
			Co2 = measure.Co2,
			Humidity = measure.Humidity,
			Temperature = Convert.ToDouble(measure.Temperature)
		};

		await responseStream.WriteAsync(response);
	}
}