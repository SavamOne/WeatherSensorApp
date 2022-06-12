using Microsoft.AspNetCore.Mvc;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Services;
using WeatherSensorApp.Server.Contracts;
using WeatherSensorApp.Server.Converters;

namespace WeatherSensorApp.Server.ApiControllers;

[Route("sensor")]
public class SensorController : ControllerBase
{
	private readonly IMeasureService measureService;

	public SensorController(IMeasureService measureService)
	{
		this.measureService = measureService;
	}

	[HttpGet("last_measure/{sensorId:guid}")]
	public MeasureResult? GetLastMeasure(Guid sensorId)
	{
		Measure? result = measureService.GetLastMeasure(sensorId);

		return result?.ConvertToPresentation();
	}
}