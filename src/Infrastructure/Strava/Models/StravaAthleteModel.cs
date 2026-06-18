using System.Text.Json.Serialization;

namespace Infrastructure.Models;

internal record StravaAthleteModel
(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("firstname")] string FirstName,
    [property: JsonPropertyName("lastname")] string LastName,
    [property: JsonPropertyName("profile")] string? Profile
);
