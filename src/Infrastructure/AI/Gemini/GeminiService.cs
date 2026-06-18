using Application.Interfaces;
using Communication.Records;
using Infrastructure.AI.Gemini.Models;
using System.Text;
using System.Text.Json;

namespace Infrastructure.AI.Gemini;
public sealed class GeminiService : ITrainingAIService
{
    private const string Model = "gemini-2.5-flash";
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(HttpClient http, string apiKey)
    {
        _httpClient = http;
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public async Task<string> GenerateTrainingPlanAsync(TrainingPlanContext context, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            system_instruction = new
            {
                parts = new[] { new { text = BuildSystemPrompt() } }
            },
            contents = new[]
            {
                new { parts = new[] { new { text = BuildPrompt(context) } } }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 8192,
                topP = 0.9
            }
        };

        var url = $"{BaseUrl}/{Model}:generateContent?key={_apiKey}";
        var json = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if(!response.IsSuccessStatusCode)
        {
           var error = await response.Content.ReadAsStringAsync(cancellationToken);
            
            throw new HttpRequestException($"Gemini API error {response.StatusCode}: {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson)?? throw new InvalidOperationException("Invalid response from Gemini API");

        return geminiResponse.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? throw new InvalidOperationException("No content returned from Gemini API");
    }

    private string BuildSystemPrompt()
    {
        return "        Você é um treinador de corrida especializado, focado em corredores iniciantes\r\n        e intermediários que querem melhorar seu desempenho de forma progressiva.\r\n        Analise os dados reais do Strava do atleta e crie planos de treino semanais\r\n        personalizados, práticos e motivadores.\r\n        Responda sempre em português brasileiro.\r\n        Para cada treino, especifique: dia da semana, tipo de sessão, duração em min, distancia total esperada e pace médio esperado,\r\n        descrição detalhada e pace sugerido em min/km. pode organizar como uma lista de topicos" +
            "\r\n";
    }

    private string BuildPrompt(TrainingPlanContext context)
    {
       return $"""
        ##Dados do Atleta
        -Nome: {context.AthleteName}
        -Nível estimado: {context.FitnessLevel}
        -Meta: {context.Goal}
        Prazo: {context.DeadLineDescription}
        Dias de treino por semana: {context.TrainingDaysWeek}x

        ##Histórico Recente de Corridas (dados reais do Strava)
        {context.RecentActivitiesSummary}

        ##Tarefa
        Com base nesses dados reais, crie um plano de treino para esta semana com {context.TrainingDaysWeek} sessões. Inclua no final uma dica de recução e nutrição para potencializar os resultados.
        """;
    }
}
