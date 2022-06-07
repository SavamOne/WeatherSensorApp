using WeatherSensorApp.Client.BackgroundServices;
using WeatherSensorApp.Client.Business.Services;
using WeatherSensorApp.Client.Business.Services.Implementations;
using WeatherSensorApp.Client.GrpcClientServices;
using WeatherSensorApp.Client.GrpcClientServices.Implementations;
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
		builder.Services.AddGrpcClient<MeasureSubscriptionService.MeasureSubscriptionServiceClient>(options =>
		{
			options.Address = new Uri("http://localhost:5002");
		});

		WebApplication app = builder.Build();

		app.MapControllers();

		app.Run();
	}
}