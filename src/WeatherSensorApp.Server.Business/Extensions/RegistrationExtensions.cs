using Microsoft.Extensions.DependencyInjection;
using WeatherSensorApp.Server.Business.Services;
using WeatherSensorApp.Server.Business.Services.Implementations;
using WeatherSensorApp.Server.Business.Storages;
using WeatherSensorApp.Server.Business.Storages.Implementations;
using WeatherSensorApp.Server.Business.Workers;

namespace WeatherSensorApp.Server.Business.Extensions;

public static class RegistrationExtensions
{
	public static void AddBusinessLogic(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<ILastMeasureStore, LastMeasureStore>();
		serviceCollection.AddSingleton<IMeasureSubscriptionStore, MeasureSubscriptionStore>();

		serviceCollection.AddSingleton<IMeasureService, MeasureService>();

		serviceCollection.AddHostedService<BackgroundMeasureService>();
	}
}