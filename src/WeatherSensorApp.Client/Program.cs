using WeatherSensorApp.Client.BackgroundServices;
using WeatherSensorApp.Client.Business.Helpers;
using WeatherSensorApp.Client.Business.Helpers.Implementations;
using WeatherSensorApp.Client.Business.Options;
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

		builder.Services.AddOptions<AggregationOptions>()
		   .Bind(builder.Configuration.GetSection(nameof(AggregationOptions)))
		   .Validate(options => options.AggregationPeriodInMinutes > 0 && 60 % options.AggregationPeriodInMinutes == 0);

		builder.Services.AddSingleton<IAggregationHelper, ByMinuteAggregationHelper>();
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