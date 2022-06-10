using WeatherSensorApp.Server.BackgroundServices;
using WeatherSensorApp.Server.Business.Extensions;
using WeatherSensorApp.Server.Business.Options;
using WeatherSensorApp.Server.GrpcServices;
using Grpc.Reflection;
using WeatherSensorApp.Server.Options;

namespace WeatherSensorApp.Server;

public static class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		builder.Services.AddOptions<MeasureOptions>()
		   .Bind(builder.Configuration.GetSection(nameof(MeasureOptions)))
		   .ValidateDataAnnotations();
		builder.Services.AddOptions<SensorOptions>()
		   .Bind(builder.Configuration.GetSection(nameof(SensorOptions)))
		   .ValidateDataAnnotations();
		builder.Services.AddHostedService<BackgroundMeasureService>();
		builder.Services.AddBusinessLogic();

		builder.Services.AddControllers();
		builder.Services.AddGrpc();
		builder.Services.AddGrpcReflection();

		WebApplication app = builder.Build();

		app.MapGrpcService<MeasureApiService>();
		app.MapControllers();
		app.MapGrpcReflectionService();

		app.Run();
	}
}