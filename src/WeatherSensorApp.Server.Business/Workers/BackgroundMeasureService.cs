using Microsoft.Extensions.Hosting;
using WeatherSensorApp.Server.Business.Contracts;
using WeatherSensorApp.Server.Business.Helpers;
using WeatherSensorApp.Server.Business.Services;

namespace WeatherSensorApp.Server.Business.Workers;

public class BackgroundMeasureService : BackgroundService
{
	private readonly IMeasureService measureService;

	public BackgroundMeasureService(IMeasureService measureService)
	{
		this.measureService = measureService;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		IReadOnlyCollection<Sensor> sensors = measureService.GetAvailableSensors();

		while (!stoppingToken.IsCancellationRequested)
		{
			foreach (Sensor sensor in sensors)
			{
				Measure randomMeasure = MeasureGenerator.GenerateRandom(sensor.Id);
				measureService.OnNewMeasure(randomMeasure);
			}

			await Task.Delay(2000, stoppingToken);
		}
	}
}