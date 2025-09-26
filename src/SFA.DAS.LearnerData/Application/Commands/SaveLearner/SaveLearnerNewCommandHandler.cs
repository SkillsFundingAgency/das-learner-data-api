using MediatR;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Messages;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.Application.Commands.SaveLearner;

public enum SaveLearnerNewResult
{
    Created,
    Updated
}

public class SaveLearnerNewCommandResponse
{
    public long Id { get; init; }
    public SaveLearnerNewResult Result { get; init; }
}

public class SaveLearnerNewCommandHandler(
    ILearnerRepository repository,
    IChangeTrackingService changeTrackingService,
    IEventPublisher eventPublisher) : IRequestHandler<SaveLearnerNewCommand, SaveLearnerNewCommandResponse>
{
    public async Task<SaveLearnerNewCommandResponse> Handle(SaveLearnerNewCommand request, CancellationToken cancellationToken)
    {
        var response = new SaveLearnerNewCommandResponse();
        var existingLearner = await repository.Get(request.Ukprn, request.Uln, cancellationToken);

        if (existingLearner == null)
        {
            response = await repository.Save(request, cancellationToken);
        }
        else
        {
            var updatedLearner = Learner.From(request);
            
            var changeSummary = changeTrackingService.DetectChanges(existingLearner, updatedLearner);
            response = await repository.Save(request, cancellationToken);
            
            if (changeSummary.HasChanges)
            {
                var @event = new LearnerDataUpdatedEvent
                {
                    LearnerId = response.Id,
                    ChangedAt = DateTime.UtcNow
                };
                
                await eventPublisher.PublishLearnerDataUpdatedEventAsync(@event);
            }
        }

        return response;
    }
}