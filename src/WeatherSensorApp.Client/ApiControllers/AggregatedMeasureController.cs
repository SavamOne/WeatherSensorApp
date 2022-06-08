using Microsoft.AspNetCore.Mvc;
using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Business.Services;
using WeatherSensorApp.Client.Converters;

namespace WeatherSensorApp.Client.ApiControllers;

[Route("aggregated_measure")]
public class AggregatedMeasureController : ControllerBase
{
	private readonly IAggregatedMeasureService aggregatedMeasureService;

	public AggregatedMeasureController(IAggregatedMeasureService aggregatedMeasureService)
	{
		this.aggregatedMeasureService = aggregatedMeasureService;
	}

	[HttpGet("from_sensor_info/{sensorId:guid}")]
	public IActionResult GetFromSensor(Guid sensorId, [FromQuery] DateTime? fromTime, [FromQuery] int? minutes)
	{
		TotalAggregatedMeasure result = aggregatedMeasureService.GetRange(sensorId, fromTime ?? DateTime.UtcNow, minutes ?? 0);
		return Ok(result.ConvertToPresentation());
	}

	[HttpGet("all_info")]
	public IActionResult GetAll()
	{
		var result = aggregatedMeasureService.GetAll();
		return Ok(result.Select(AggregatedMeasureConverter.ConvertToPresentation));
	}

	// TODO хочу подписываться и отписываться сразу на несколько штук
	[HttpPost("sensor_subscription/{sensorId:guid}")]
	public IActionResult SubscribeToSensor(Guid sensorId)
	{
		aggregatedMeasureService.SubscribeSensor(sensorId);
		return Ok();
	}

	[HttpDelete("sensor_subscription/{sensorId:guid}")]
	public IActionResult UnsubscribeFromSensor(Guid sensorId)
	{
		aggregatedMeasureService.UnsubscribeSensor(sensorId);
		return Ok();
	}
}