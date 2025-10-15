using MediatR;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Messages;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.Application.Commands.SaveLearner;

public enum SaveLearnerResult
{
    Created,
    Updated
}

public class SaveLearnerCommandResponse
{
    public long Id { get; init; }
    public SaveLearnerResult Result { get; init; }
}

public class SaveLearnerCommandHandler(
    ILearnerRepository repository,
    IChangeTrackingService changeTrackingService,
    IEventPublisher eventPublisher) : IRequestHandler<SaveLearnerCommand, SaveLearnerCommandResponse>
{
    public async Task<SaveLearnerCommandResponse> Handle(SaveLearnerCommand request, CancellationToken cancellationToken)
    {
        var response = new SaveLearnerCommandResponse();
        var existingLearner = await repository.Get(request.Ukprn, request.Uln, request.StandardCode, request.AcademicYear, cancellationToken);

        if (existingLearner == null)
        {
            response = await repository.Save(request, cancellationToken);
        }
        else
        {
            var updatedLearner = Learner.From(request);
            
            var changeSummary = changeTrackingService.DetectChanges(existingLearner, updatedLearner);
            response = await repository.Save(request, cancellationToken);
            
            if (changeSummary.HasLearnerChanges)
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