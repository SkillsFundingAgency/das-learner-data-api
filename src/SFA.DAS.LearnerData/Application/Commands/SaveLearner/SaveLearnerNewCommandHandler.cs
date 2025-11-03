using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Messages;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.Application.Commands.SaveLearner;

public enum SaveLearnerNewResult
{
    Created,
    Updated,
    NotNeeded
}

public class SaveLearnerNewCommandResponse
{
    public long Id { get; init; }
    public SaveLearnerNewResult Result { get; init; }
}

public class SaveLearnerNewCommandHandler(
    ILearnerRepository repository,
    IChangeTrackingService changeTrackingService,
    IEventPublisher eventPublisher, ILogger<SaveLearnerNewCommandHandler> logger) : IRequestHandler<SaveLearnerNewCommand, SaveLearnerNewCommandResponse>
{
    public async Task<SaveLearnerNewCommandResponse> Handle(SaveLearnerNewCommand request, CancellationToken cancellationToken)
    {
        var existingLearner = await repository.Get(request.Ukprn, request.Uln, cancellationToken);

        if (existingLearner == null)
        {
            return await repository.AddLearner(request, cancellationToken);
        }

        var updatedLearner = Learner.From(request);
        var changeSummary = changeTrackingService.DetectChanges(existingLearner, updatedLearner);

        if (ApprovedLearnerRecordHasBeenMateriallyUpdated())
        {
            return await repository.AddLearner(request, cancellationToken);
        }
        if (ApprovedLearnerRecordHasNotBeenMateriallyUpdated())
        {
            logger.LogInformation("An identical approved learner record already exists for learner id {0}", existingLearner.Id);
            return new SaveLearnerNewCommandResponse {Id = existingLearner.Id, Result = SaveLearnerNewResult.NotNeeded};
        }

        var response = await repository.UpdateLearner(existingLearner, request, cancellationToken);

        if (changeSummary.HasLearnerChanges)
        {
            var @event = new LearnerDataUpdatedEvent
            {
                LearnerId = response.Id,
                ChangedAt = DateTime.UtcNow
            };

            await eventPublisher.PublishLearnerDataUpdatedEventAsync(@event);
        }

        return response;

        bool ApprovedLearnerRecordHasBeenMateriallyUpdated() => changeSummary.HasMaterialChanges && existingLearner.ApprenticeshipId != null;
        bool ApprovedLearnerRecordHasNotBeenMateriallyUpdated() => !changeSummary.HasMaterialChanges && existingLearner.ApprenticeshipId != null;
    }
}