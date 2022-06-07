using WeatherSensorApp.Client.BackgroundServices;
using WeatherSensorApp.Client.Business.Services;
using WeatherSensorApp.Client.Business.Services.Implementations;
using WeatherSensorApp.Client.GrpcClientServices;
using WeatherSensorApp.Client.GrpcClientServices.Implementations;
using WeatherSensorApp.Client.Options;
using WeatherSensorApp.Server;

namespace WeatherSensorApp.Client;

public static class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddSingleton<IMeasureApiClientService, MeasureApiClientService>();
		builder.Services.AddSingleton<IAggregatedMeasureService, AggregatedMeasureService>();
		builder.Services.AddHostedService<BackgroundMeasureSubscriptionService>();

		ServerAddressOptions addressOptions = new();
		builder.Configuration.GetSection(nameof(ServerAddressOptions)).Bind(addressOptions);
		
		builder.Services.AddGrpcClient<MeasureSubscriptionService.MeasureSubscriptionServiceClient>(options =>
		{
			options.Address = new Uri(addressOptions.ServerAddress);
		});

		WebApplication app = builder.Build();

		app.MapControllers();

		app.Run();
	}
}