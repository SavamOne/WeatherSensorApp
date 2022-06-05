using Microsoft.AspNetCore.Mvc;
using WeatherSensorApp.Business.Contracts;
using WeatherSensorApp.Server.Business.Services;
using WeatherSensorApp.Server.Contracts;

namespace WeatherSensorApp.Server.ApiControllers;

[Route("[controller]")]
public class SensorController : ControllerBase
{
	private readonly IMeasureService measureService;
	
	public SensorController(IMeasureService measureService) 
	{
		this.measureService = measureService;
	}

	[HttpGet("last_measure/{sensorId:guid}")]
	public IActionResult GetLastMeasure(Guid sensorId)
	{
		Measure? result = measureService.GetLastMeasure(sensorId);

		if (result is null)
		{
			return Ok(null);
		}

		return Ok(new MeasureResult
		{
			Co2 = result.Co2,
			Humidity = result.Co2,
			Temperature = result.Temperature,
			MeasureTime = result.MeasureTime,
			SensorId = result.SensorId
		});
	}
}