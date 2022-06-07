using Microsoft.AspNetCore.Mvc;
using WeatherSensorApp.Client.Converters;
using WeatherSensorApp.Client.GrpcClientServices;

namespace WeatherSensorApp.Client.ApiControllers;

[Route("sensor")]
public class SensorController : ControllerBase
{
	private readonly IMeasureApiClientService clientService;

	public SensorController(IMeasureApiClientService clientService)
	{
		this.clientService = clientService;
	}
	
	[HttpGet("available_sensors")]
	public async Task<IActionResult> GetAvailableSensors()
	{
		var result= await clientService.GetAvailableSensorsAsync();

		return Ok(result.Select(SensorConverter.ConvertToPresentation));
	}
}