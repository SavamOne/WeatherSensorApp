using Microsoft.AspNetCore.Mvc;
using WeatherSensorApp.Client.Business.Contracts;
using WeatherSensorApp.Client.Business.Services;
using WeatherSensorApp.Client.Contracts;
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
	public TotalAggregatedMeasureResult GetFromSensor(Guid sensorId, [FromQuery] DateTime? fromTime, [FromQuery] int? periods)
	{
		TotalAggregatedMeasure result = aggregatedMeasureService.GetRange(sensorId, fromTime ?? DateTime.UtcNow, periods ?? 1);
		return result.ConvertToPresentation();
	}

	[HttpGet("all_info")]
	public IEnumerable<AggregatedMeasureResult> GetAll()
	{
		IReadOnlyCollection<AggregatedMeasure> result = aggregatedMeasureService.GetAll();
		return result.Select(AggregatedMeasureConverter.ConvertToPresentation);
	}

	// TODO хочу подписываться и отписываться сразу на несколько штук
	[HttpPost("sensor_subscription/{sensorId:guid}")]
	public void SubscribeToSensor(Guid sensorId)
	{
		aggregatedMeasureService.SubscribeSensor(sensorId);
	}

	[HttpDelete("sensor_subscription/{sensorId:guid}")]
	public void UnsubscribeFromSensor(Guid sensorId)
	{
		aggregatedMeasureService.UnsubscribeSensor(sensorId);
	}
}