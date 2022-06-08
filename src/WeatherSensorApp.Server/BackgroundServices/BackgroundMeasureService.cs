using Microsoft.Extensions.Options;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Helpers;
using WeatherSensorApp.Server.Business.Options;
using WeatherSensorApp.Server.Business.Services;

namespace WeatherSensorApp.Server.BackgroundServices;

public class BackgroundMeasureService : BackgroundService
{
	// TODO а почему эти параметры не вынести в атрибут Range прям на самой модели конфигурации он сразу может провалидировать при старте и упасть при ошибочных параметрах
	// TODO а так получается что человек введет значения и программа продолжит работать даже с 10000000 интервалом
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

		// TODO а почему бы их параллельно не запустить
		// TODO а то таким образом у тебя получается задержка)) и то что они по очереди идут
		// TODO а если их сотни, и отдают много данных, можно тогда уйти за лимиты по времени ответа от каждого датчика
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