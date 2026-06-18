using Application.Interfaces;
using Communication.Records;
using Communication.Request;
using Communication.Response;

namespace Application.UseCases;
public sealed class GenerateWeeklyPlanUseCase
{
    private readonly IStravaService _stravaService;
    private readonly ITrainingAIService _trainingAIService;

    public GenerateWeeklyPlanUseCase(ITrainingAIService aiService, IStravaService stravaService)
    {
        _stravaService = stravaService;
        _trainingAIService = aiService;
    }

    public async Task<GenerateWeeklyPlanResponse> Execute(GenerateWeeklyPlanRequest request ,CancellationToken cancellationToken =default)
    {
        var athlete = await _stravaService.GetAthleteAsync(request.AccessToken, cancellationToken);
        var activities = await _stravaService.GetRecentActivitiesAsync(request.AccessToken);

        athlete.AddActivities(activities);

        var context = new TrainingPlanContext
        (
            AthleteName: athlete.FullName,
            FitnessLevel: athlete.FitnessLevel.ToString(),
            Goal: request.Goal,
            DeadLineDescription: request.DeadlineDescription,
            RecentActivitiesSummary: athlete.GetActivitySummary(),
            TrainingDaysWeek: request.TrainingDaysPerWeek
        );

        var rawPlan = await _trainingAIService.GenerateTrainingPlanAsync(context, cancellationToken);

        return new GenerateWeeklyPlanResponse(athlete, rawPlan);
    }
}
