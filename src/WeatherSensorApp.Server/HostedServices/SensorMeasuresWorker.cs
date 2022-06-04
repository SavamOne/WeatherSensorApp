using WeatherSensorApp.Server.Business.Contracts;
using WeatherSensorApp.Server.Business.Helpers;
using WeatherSensorApp.Server.Business.Services.Implementations;

namespace WeatherSensorApp.Server.HostedServices;

public class SensorMeasuresWorker : BackgroundService
{
	private readonly ILogger<SensorMeasuresWorker> logger;
	private readonly MeasureService measureService;

	public SensorMeasuresWorker(MeasureService measureService, ILogger<SensorMeasuresWorker> logger)
	{
		this.measureService = measureService;
		this.logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var sensors = measureService.GetAvailableSensors();

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