namespace Communication.Request;
public record GenerateWeeklyPlanRequest
(
    string AccessToken,
    string Goal,
    string DeadlineDescription,
    int TrainingDaysPerWeek = 3
);
