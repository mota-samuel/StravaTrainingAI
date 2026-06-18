using Communication.Records;

namespace Application.Interfaces;
public interface ITrainingAIService
{
    Task<string> GenerateTrainingPlanAsync(TrainingPlanContext context, CancellationToken cancellationToken = default);
}
