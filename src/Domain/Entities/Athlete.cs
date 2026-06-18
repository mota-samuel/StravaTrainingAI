using Domain.Enum;

namespace Domain.Entities;
public sealed class Athlete
{
    public long Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? ProfilePictureUrl { get; private set; }
    public FitnessLevel FitnessLevel { get; private set; }

    private readonly List<Activity> _activities = new();
    public IReadOnlyList<Activity> Activities => _activities.AsReadOnly();

    private Athlete() { }

    public static Athlete Create(
        long id, string fName, string lName, string? profilePictureUrl = null, FitnessLevel fitnessLevel = FitnessLevel.Beginner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lName);

        return new Athlete
        {
            Id = id,
            FirstName = fName,
            LastName = lName,
            ProfilePictureUrl = profilePictureUrl,
            FitnessLevel = fitnessLevel
        };
    }

    public void AddActivities(IEnumerable<Activity> activities)
    {
        foreach (var a in activities) _activities.Add(a);
        RecalculateFitnessLevel();
    }

    public string FullName => $"{FirstName} {LastName}";

    private void RecalculateFitnessLevel()
    {
        var recentRuns = _activities
            .Where(a => a.Type == ActivityType.Run)
            .OrderByDescending(a => a.StartDate)
            .Take(10).ToList();

        if (!recentRuns.Any())
        {
            FitnessLevel = FitnessLevel.Beginner; return;
        }

        var avgPace = recentRuns
            .Where(a => a.Pace is not null)
            .Average(a => a.Pace!.SecondesPerKm);

        FitnessLevel = avgPace switch
        {
            < 330 => FitnessLevel.Advanced,
            < 390 => FitnessLevel.Intermediate,
            _ => FitnessLevel.Beginner
        };
    }

    public string GetActivitySummary(int lastN = 10)
    {
        var recentRuns = _activities
            .Where(a => a.Type == ActivityType.Run)
            .OrderByDescending(a => a.StartDate)
            .Take(lastN).ToList();

        if (!recentRuns.Any())
        {
            return "Nenhuma corrida recente encontrada!";
        }

        return string.Join(Environment.NewLine, recentRuns.Select(a => $"- {a.StartDate:dd/MM/yy}: {a.Distance.InKilometers:F1}Km " + (a.Pace is not null ? $" (pace: {a.Pace})" : "") + (a.AverageHeartRate.HasValue ? $" | FC: {a.AverageHeartRate}bpm" : "")
            ));
    }
}
