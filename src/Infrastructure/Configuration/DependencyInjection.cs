using Application.Interfaces;
using Application.UseCases;
using Communication.Records;
using Infrastructure.AI.Gemini;
using Infrastructure.Strava;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Infrastructure.Configuration;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, StravaOptions stravaOptions, string geminiApiKey)
    {
        services.AddHttpClient<IStravaService, StravaService>()
            .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(30));

        services.AddHttpClient<ITrainingAIService, GeminiService>()
            .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(60));
        
        services.AddSingleton(stravaOptions);
        services.AddSingleton(geminiApiKey);

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<GenerateWeeklyPlanUseCase>();
        return services;
    }


}
