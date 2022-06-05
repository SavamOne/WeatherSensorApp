using Grpc.Core;
using WeatherSensorApp.Server;

namespace WeatherSensorApp.Client.Workers;

public class BackgroundMeasureSubscriptionService : BackgroundService
{
	private readonly IServiceProvider serviceProvider;
	private readonly ILogger<BackgroundMeasureSubscriptionService> logger;

	public BackgroundMeasureSubscriptionService(IServiceProvider serviceProvider, ILogger<BackgroundMeasureSubscriptionService> logger)
	{
		this.serviceProvider = serviceProvider;
		this.logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using IServiceScope scope = serviceProvider.CreateScope();
		MeasureSubscriptionService.MeasureSubscriptionServiceClient client = scope.ServiceProvider.GetRequiredService<MeasureSubscriptionService.MeasureSubscriptionServiceClient>();
		
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await ProcessStreams(client, stoppingToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Unknown exception while processing grpc call");
			}
		}
	}

	private async Task ProcessStreams(MeasureSubscriptionService.MeasureSubscriptionServiceClient client, CancellationToken stoppingToken)
	{
		using AsyncDuplexStreamingCall<MeasureRequest, MeasureResponse> call = client.StreamMeasures(Metadata.Empty, cancellationToken: stoppingToken);

		Task readTask = Task.Run(async () =>
		{
			await foreach(MeasureResponse response in call.ResponseStream.ReadAllAsync(stoppingToken))
			{
				logger.LogWarning("{0}: Id: {1}, Temp: {2}, Hum: {3},  Co2: {4}",
					response.Time?.ToDateTime(),
					response.SensorId,
					response.Temperature,
					response.Humidity,
					response.Co2);
			}

			logger.LogWarning("readTaskCompleted");
		}, stoppingToken);

		try
		{
			foreach (string id in new[] { "9295d744-192f-42f4-86eb-082e5510846b", "b4fcab16-c6a1-4f5b-9b54-b3be516c249a", "58edced9-fdb7-49c4-be20-76316037ec20" }) 
			{
				await call.RequestStream.WriteAsync(new MeasureRequest
				{
					SensorId = id,
					Subscribe = true
				}, stoppingToken);
			}

			await readTask;
		}
		finally
		{
			await call.RequestStream.CompleteAsync();
			
			logger.LogWarning("Completed And Disposed");
		}
	}
}