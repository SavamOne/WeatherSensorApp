using WeatherSensorApp.Business.Contracts;

namespace WeatherSensorApp.Client.GrpcClientServices;

public interface IMeasureApiClientService
{
	Task RunMeasureStreamAsync(CancellationToken stoppingToken);

	Task<IReadOnlyCollection<Sensor>> GetAvailableSensorsAsync();
}