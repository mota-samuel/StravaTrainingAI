using Domain.Enum;
using Domain.ValueObjects;

namespace Domain.Entities;
public class Activity
{
    public long Id { get; set; }
    public string Name { get; private set; }
    public ActivityType Type { get; private set; }
    public Distance Distance { get; private set; } = null!;
    public TimeSpan MovingTime { get; private set; }
    public DateTime StartDate { get; private set; }
    public Pace? Pace { get; private set; }
    public int? AverageHeartRate { get; private set; }
    public int? MaximumHeartRate { get; private set; }
    public double? ElevationGain { get; private set; }

    private Activity() { }

    public static Activity Create(
        long id, string name, ActivityType type, double distanceInMeters, int movingTimeSeconds, DateTime startDate, int? averageHR = null, int? maxHR = null, double? elevationGain = null)
    {
        var distance = Distance.FromMeters(distanceInMeters);
        var movingTime = TimeSpan.FromSeconds(movingTimeSeconds);

        Pace? pace = null;
        if(type.Equals(ActivityType.Run) && distanceInMeters > 0)
            pace = Pace.Calculate(distance, movingTime);

        return new Activity
        {
            Id = id,
            Name = name,
            Type = type,
            Distance = distance,
            MovingTime = movingTime,
            StartDate = startDate,
            Pace = pace,
            AverageHeartRate = averageHR,
            MaximumHeartRate = maxHR,
            ElevationGain = elevationGain
        };
    }
}
