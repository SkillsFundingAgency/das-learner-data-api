using MediatR;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Commands;

public class CreateLearnerCommandHandler(ILearnerDataRepository repository) : IRequestHandler<CreateLearnerCommand>
{
    public async Task Handle(CreateLearnerCommand request, CancellationToken cancellationToken)
    {
        var learner = Learner.From(request);
        
        await repository.Create(learner);
    }
}