using System.Text.Json.Serialization;

namespace Infrastructure.Models;
internal record StravaActivityModel
(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("sport_type")] string SportType,
    [property: JsonPropertyName("distance")] double Distance,
    [property: JsonPropertyName("moving_time")] int MovingTime,
    [property: JsonPropertyName("start_date_local")] DateTime StartDateLocal,
    [property: JsonPropertyName("average_heartrate")] double? AverageHeartrate,
    [property: JsonPropertyName("max_heartrate")] double? MaxHeartrate,
    [property: JsonPropertyName("total_elevation_gain")] double? TotalElevationGain
);
