using Application.Interfaces;
using Communication.Records;
using Communication.Response;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Infrastructure.Strava;
public sealed class StravaService : IStravaService
{
    private const string BaseUrl = "https://www.strava.com/api/v3";
    private const string AuthUrl = "https://www.strava.com/oauth/authorize";
    private const string TokenUrl = "https://www.strava.com/oauth/token";

    private readonly HttpClient _httpClient;
    private readonly StravaOptions _options;

    public StravaService(HttpClient http, StravaOptions opt)
    {
        _httpClient = http;
        _options = opt;
    }

    public Task<string> GetAuhorizationUrl()
    {
        var url = $"{AuthUrl}?client_id={_options.ClientId}" +
                  $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
                  $"&response_type=code&approval_prompt=auto" +
                  $"&scope=read,activity:read_all";
        return Task.FromResult(url);
    }

    public async Task<StravaTokens> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default)
    {
        var payload = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code"
        });

        var response = await _httpClient.PostAsync(TokenUrl, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var token = JsonSerializer.Deserialize<StravaTokenResponse>(await response.Content.ReadAsStringAsync(cancellationToken))
            ?? throw new InvalidOperationException("Failed to deserialize Strava token response.");
        return new StravaTokens(token.AccessToken, token.RefreshToken, token.ExpiresAt);
    }

    public async Task<Athlete> GetAthleteAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.SendAsync(
            CreateRequest(HttpMethod.Get, "/athlete", accessToken),cancellationToken);
        response.EnsureSuccessStatusCode();

        var model = JsonSerializer.Deserialize<StravaAthleteModel>(await response.Content.ReadAsStringAsync(cancellationToken))
            ?? throw new InvalidOperationException("Failed to deserialize Strava athlete response.");
        return Athlete.Create(model.Id, model.FirstName, model.LastName, model.Profile);
    }

    

    public Task<string> GetAthorizationUrlAsync()
    {
        var url = $"{AuthUrl}?client_id={_options.ClientId}" +
          $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
          $"&response_type=code&approval_prompt=auto" +
          $"&scope=read,activity:read_all";
        return Task.FromResult(url);

    }

    public async Task<IReadOnlyList<Activity>> GetRecentActivitiesAsync(string accessToken, int perPage = 20, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.SendAsync(
            CreateRequest(HttpMethod.Get, $"/athlete/activities?per_page={perPage}", accessToken), cancellationToken);
        response.EnsureSuccessStatusCode();

        var models = JsonSerializer.Deserialize<List<StravaActivityModel>>(await response.Content.ReadAsStringAsync(cancellationToken))
            ?? new();

        return models.Select(model => Activity.Create(
            id: model.Id,
            name: model.Name,
            type: MapActivityType(model.SportType),
            distanceInMeters: model.Distance,
            movingTimeSeconds: model.MovingTime,
            startDate: model.StartDateLocal,
            averageHR: model.AverageHeartrate.HasValue ? (int)model.AverageHeartrate : null,
            maxHR: model.MaxHeartrate.HasValue ? (int)model.MaxHeartrate : null,
            elevationGain: model.TotalElevationGain.HasValue ? (int)model.TotalElevationGain : null
        )).ToList().AsReadOnly();
    }

    public async Task<StravaTokens> RefreshTokensAsync(
    string refreshToken, CancellationToken ct = default)
    {
        var payload = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["refresh_token"] = refreshToken,
            ["grant_type"] = "refresh_token"
        });

        var response = await _httpClient.PostAsync(TokenUrl, payload, ct);
        response.EnsureSuccessStatusCode();

        var token = JsonSerializer.Deserialize<StravaTokenResponse>(
            await response.Content.ReadAsStringAsync(ct))!;

        return new StravaTokens(token.AccessToken, token.RefreshToken, token.ExpiresAt);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path, string accessToken)
    {
        var requestHttp = new HttpRequestMessage(method, $"{BaseUrl}{path}");
        requestHttp.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return requestHttp;
    }

    private static ActivityType MapActivityType(string type) => type.ToLower() switch
    {
        "run" or "trailrun" or "virtualrun" => ActivityType.Run,
        "ride" or "virtualride" => ActivityType.Ride,
        "swim" => ActivityType.Swim,
        "walk" or "hike" => ActivityType.Walk,
        _ => ActivityType.Other
    };
}
