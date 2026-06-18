using Domain.Entities;

namespace Communication.Response;
public record GenerateWeeklyPlanResponse
(
    Athlete Athlete,
    string rawPlanFromAI
);
