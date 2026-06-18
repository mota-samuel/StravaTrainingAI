namespace Communication.Records;
public record StravaTokens(string AccessToken,
    string RefreshToken,
    long ExpiresAt
);
