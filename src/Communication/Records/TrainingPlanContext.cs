namespace Communication.Records;
public record TrainingPlanContext (
    string AthleteName,
    string FitnessLevel,
    string Goal,
    string DeadLineDescription,
    string RecentActivitiesSummary,
    int TrainingDaysWeek
);

