using System.Text.Json.Serialization;

namespace Infrastructure.AI.Gemini.Models;
internal record GeminiResponse
(
    [property: JsonPropertyName("candidates")] List<GeminiCandidate>? Candidates
);
    