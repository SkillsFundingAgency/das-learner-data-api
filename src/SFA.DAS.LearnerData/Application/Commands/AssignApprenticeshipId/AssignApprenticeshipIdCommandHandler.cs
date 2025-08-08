using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;

public class AssignApprenticeshipIdCommandHandler(ILearnerRepository repository) : IRequestHandler<AssignApprenticeshipIdCommand>
{
    public Task Handle(AssignApprenticeshipIdCommand request, CancellationToken cancellationToken)
    {
        return repository.AssignApprenticeshipId(request, cancellationToken);
    }
}