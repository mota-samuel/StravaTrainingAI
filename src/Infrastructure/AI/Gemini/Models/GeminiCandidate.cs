using System.Text.Json.Serialization;

namespace Infrastructure.AI.Gemini.Models;
internal record GeminiCandidate
(
    [property: JsonPropertyName("content")] GeminiContent? Content
);
