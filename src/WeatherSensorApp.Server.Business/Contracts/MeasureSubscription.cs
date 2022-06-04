namespace WeatherSensorApp.Server.Business.Contracts;

public class MeasureSubscription : IEquatable<MeasureSubscription>
{
	public MeasureSubscription(Guid id,
		Guid sensorId,
		CancellationToken cancellationToken,
		Func<Measure, Task> callback)
	{
		Id = id;
		SensorId = sensorId;
		CancellationToken = cancellationToken;
		Callback = callback;
	}

	public Guid Id { get; }

	public Guid SensorId { get; }

	public CancellationToken CancellationToken { get; }

	public Func<Measure, Task> Callback { get; }

	public bool Equals(MeasureSubscription? other)
	{
		if (ReferenceEquals(null, other))
		{
			return false;
		}
		if (ReferenceEquals(this, other))
		{
			return true;
		}
		return Id.Equals(other.Id)
			&& SensorId.Equals(other.SensorId);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}
		if (ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((MeasureSubscription)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, SensorId);
	}
}