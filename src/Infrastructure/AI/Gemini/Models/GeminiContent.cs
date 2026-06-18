using System.Text.Json.Serialization;

namespace Infrastructure.AI.Gemini.Models;
internal record GeminiContent
(
    [property: JsonPropertyName("parts")] List<GeminiPart>? Parts
);

