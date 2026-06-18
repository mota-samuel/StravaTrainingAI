using Communication.Records;
using Domain.Entities;

namespace Application.Interfaces;
public interface IStravaService
{
    Task<string> GetAthorizationUrlAsync();
    Task<StravaTokens> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default);
    Task<Athlete> GetAthleteAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Activity>> GetRecentActivitiesAsync(string accessToken, int perPage = 20, CancellationToken cancellationToken = default);
    Task<StravaTokens> RefreshTokensAsync(string refreshToken, CancellationToken ct = default);
}
