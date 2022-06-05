using WeatherSensorApp.Client.Workers;
using WeatherSensorApp.Server;

namespace WeatherSensorApp.Client;

public static class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		
		builder.Services.AddControllers();
		builder.Services.AddHostedService<BackgroundMeasureSubscriptionService>();
		builder.Services.AddGrpcClient<MeasureSubscriptionService.MeasureSubscriptionServiceClient>(options =>
		{
			options.Address = new Uri("https://localhost:7133");
		});

		WebApplication app = builder.Build();
		
		app.MapControllers();

		app.Run();
	}
}