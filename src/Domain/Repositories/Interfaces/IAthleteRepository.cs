using Domain.Entities;

namespace Domain.Repositories.Interfaces;
public interface IAthleteRepository
{
    Task<Athlete?> GetByIdAsync(long athleteId, CancellationToken cancellationToken = default);
    Task SaveAsync(Athlete athlete, CancellationToken cancellationToken = default);
}
