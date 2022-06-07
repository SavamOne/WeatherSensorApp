using Microsoft.Extensions.Options;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Helpers;
using WeatherSensorApp.Server.Business.Options;
using WeatherSensorApp.Server.Business.Services;

namespace WeatherSensorApp.Server.BackgroundServices;

public class BackgroundMeasureService : BackgroundService
{
	private const int MinInterval = 100;
	private const int MaxInterval = 2000;
	private readonly int interval;

	private readonly IMeasureService measureService;

	public BackgroundMeasureService(IMeasureService measureService, IOptions<MeasureOptions> options)
	{
		this.measureService = measureService;
		interval = Math.Min(Math.Max(options.Value.IntervalInMillis, MinInterval), MaxInterval);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		IReadOnlyCollection<Sensor> sensors = measureService.GetAvailableSensors();

		while (!stoppingToken.IsCancellationRequested)
		{
			Task delay = Task.Delay(interval, stoppingToken);

			foreach (Sensor sensor in sensors)
			{
				Measure randomMeasure = MeasureGenerator.GenerateRandom(sensor.Id);
				measureService.OnNewMeasure(randomMeasure);
			}

			await delay;
		}
	}
}