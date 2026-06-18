using Application.Interfaces;
using Application.UseCases;
using Communication.Records;
using Communication.Request;
using DotNetEnv;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Helpers;
using System.Text.Json;


Console.OutputEncoding = System.Text.Encoding.UTF8;
Env.Load(path:"C:\\Users\\samur\\source\\repos\\Workspace\\StravaTrainingAI\\.env");
Helpers.PrintBanner();

var stravaClientId = Helpers.GetEnvRequired("STRAVA_CLIENT_ID");
var stravaClientSecret = Helpers.GetEnvRequired("STRAVA_CLIENT_SECRET");
var geminiApiKey = Helpers.GetEnvRequired("GEMINI_API_KEY");


const string RedirectUri = "http://localhost:5000/callback/";
var stravaOptions = new StravaOptions
(
    stravaClientId,
    stravaClientSecret,
    RedirectUri
);

//DI Container - Composition Root
var services = new ServiceCollection()
    .AddInfrastructure(stravaOptions, geminiApiKey)
    .AddApplication()
    .BuildServiceProvider();

//OAuth Strava

var stravaService = services.GetRequiredService<IStravaService>();

// ── 3. OAuth com Strava ───────────────────────────────────────────
const string TokenFile = "strava_tokens.json";
StravaTokens tokens;
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

if (File.Exists(TokenFile))
{
    Console.WriteLine("🔑 Renovando token salvo...");
    var saved = JsonSerializer.Deserialize<StravaTokens>(
        await File.ReadAllTextAsync(TokenFile))!;
    tokens = await stravaService.RefreshTokensAsync(saved.RefreshToken, cts.Token);
}
else
{
    Console.WriteLine("🔗 Primeiro acesso — abrindo navegador para autorização...");
    var authUrl = await stravaService.GetAthorizationUrlAsync();
    Helpers.OpenBrowser(authUrl);

    using var cbServer = new OAuthCallbackServer(RedirectUri);
    cbServer.Start();
    var code = await cbServer.WaitForCodeAsync(cts.Token);
    tokens = await stravaService.ExchangeCodeForTokenAsync(code, cts.Token);
    Console.WriteLine("✅ Autorizado!");
}

await File.WriteAllTextAsync(TokenFile, JsonSerializer.Serialize(tokens));

Console.WriteLine("Analisando suas corridas e gerando plano com Gemini AI...");

var useCase = services.GetRequiredService<GenerateWeeklyPlanUseCase>();
var response = await useCase.Execute(new GenerateWeeklyPlanRequest(tokens.AccessToken, "Correr 5Km em menos de 20min.", "até agosto de 2025"), cts.Token);

Console.Clear();
Helpers.PrintBanner();

Console.WriteLine($"\n Olá, {response.Athlete.FullName}!");
Console.WriteLine($"Nível atual: {response.Athlete.FitnessLevel}");
Console.WriteLine($" Atividades analisadas: {response.Athlete.Activities.Count}\n");
Console.WriteLine(new string('═', 62));
Console.WriteLine("               PLANO DE TREINO SEMANAL");
Console.WriteLine(new string('═', 62));
Console.WriteLine();
Console.WriteLine(response.rawPlanFromAI
    );
Console.WriteLine();
Console.WriteLine(new string('═', 62));
Console.WriteLine("Boa semana de treinos! Execute novamente segunda-feira.");

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();