using Microsoft.Extensions.Options;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Helpers;
using WeatherSensorApp.Server.Business.Services;
using WeatherSensorApp.Server.Options;

namespace WeatherSensorApp.Server.BackgroundServices;

public class BackgroundMeasureService : BackgroundService
{
	private int interval;
	
	private readonly IMeasureService measureService;

	private readonly IDisposable changeMonitor;

	public BackgroundMeasureService(IMeasureService measureService, IOptionsMonitor<MeasureOptions> options)
	{
		this.measureService = measureService;
		interval = options.CurrentValue.IntervalInMillis;
		changeMonitor = options.OnChange(value => interval = value.IntervalInMillis);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		IReadOnlyCollection<Sensor> sensors = measureService.GetAvailableSensors();
		
		while (!stoppingToken.IsCancellationRequested)
		{
			Task delay = Task.Delay(interval, stoppingToken);
			
			foreach (Sensor sensor in sensors)
			{
				_ = Task.Run(() =>
				{
					Measure randomMeasure = MeasureGenerator.GenerateRandom(sensor.Id);
					measureService.OnNewMeasure(randomMeasure);
				}, stoppingToken);
			}

			await delay;
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		changeMonitor.Dispose();
	}
}