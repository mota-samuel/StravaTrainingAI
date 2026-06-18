using System.Text.Json.Serialization;

namespace Infrastructure.AI.Gemini.Models;
internal record GeminiPart
(
    [property: JsonPropertyName("text")] string Text
);
