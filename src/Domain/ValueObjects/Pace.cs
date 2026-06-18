using System.Data;

namespace Domain.ValueObjects;
public sealed class Pace : IEquatable<Pace>
{
    public int SecondesPerKm { get; }

    private Pace(int secondsPerKm)
    {
        SecondesPerKm = secondsPerKm;
    }

    public static Pace FromSecondsPerKM(int seconds)
    {
        if (seconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds));
        }
        return new Pace(seconds);
    }

    public static Pace Calculate(Distance distance, TimeSpan duration)
    {
        if(distance.InKilometers <= 0)
        {
            throw new ArgumentException("Distance must be  greater than zero!");
        }
        var secondsPerKm = (int)(duration.TotalSeconds / distance.InKilometers);
        return new Pace(secondsPerKm);
    }

    public int Minutes => SecondesPerKm / 60;
    public int Seconds => SecondesPerKm % 60;

    public override string ToString() => $"{Minutes}:{Seconds:D2}/Km";

    public bool Equals(Pace? other)=> other is not null && SecondesPerKm == other.SecondesPerKm;
    public override bool Equals(object? obj) => obj is Pace p && Equals(p);
    public override int GetHashCode() => SecondesPerKm.GetHashCode();
}
