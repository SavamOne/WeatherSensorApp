using WeatherSensorApp.Server.Business.Extensions;
using WeatherSensorApp.Server.GrpcServices;

namespace WeatherSensorApp.Server;

public static class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		builder.Services.AddBusinessLogic();

		builder.Services.AddControllers();
		builder.Services.AddGrpc();

		WebApplication app = builder.Build();

		app.MapGrpcService<MeasureApiService>();
		app.MapControllers();

		app.Run();
	}
}