using WeatherSensorApp.Client.GrpcClientServices;

namespace WeatherSensorApp.Client.BackgroundServices;

public class BackgroundMeasureSubscriptionService : BackgroundService
{
	private readonly IMeasureApiClientService apiClientService;
	
	public BackgroundMeasureSubscriptionService(IMeasureApiClientService apiClientService)
	{
		this.apiClientService = apiClientService;
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		return apiClientService.RunMeasureStreamAsync(stoppingToken);
	}
}