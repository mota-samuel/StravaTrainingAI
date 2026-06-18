using System.Diagnostics;

namespace Domain.Repositories.Interfaces;
public interface IActivitiesRepository
{
    Task<IReadOnlyList<Activity>> GetRecentAsync(long athleteId, int count =20, CancellationToken cancellationToken = default);
    Task SaveManyAsync(long athleteId, IEnumerable<Activity> activities, CancellationToken cancellationToken = default);
}
