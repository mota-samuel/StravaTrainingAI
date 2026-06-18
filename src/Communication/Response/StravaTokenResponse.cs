using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Infrastructure")]

namespace Communication.Response;
internal record StravaTokenResponse
(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonPropertyName("expires_at")] long ExpiresAt
);
