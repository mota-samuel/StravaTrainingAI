namespace Domain.ValueObjects;
public sealed class Distance : IEquatable<Distance>
{
    private readonly double _meters;
    private Distance(double meters)
    {
        _meters = meters;
    }

    public static Distance FromMeters(double meters)
    {
        if(meters < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(meters));
        }
        return new Distance(meters);
    }

    public static Distance FromKm(double km)
    {
       return FromMeters(km * 1000);

    }

    public double InMeters => _meters;
    public double InKilometers => _meters/1000;

    public override string ToString()
    {
        return InKilometers >= 1 ? $"{InKilometers:F1} Km" : $"{InMeters:F0} m";
    }

    public bool Equals(Distance? other) => other is not null && Math.Abs(_meters - other._meters) < 0.01;

    public override bool Equals(object? obj) => obj is Distance d && Equals(d);

    public override int GetHashCode() => _meters.GetHashCode();
}
