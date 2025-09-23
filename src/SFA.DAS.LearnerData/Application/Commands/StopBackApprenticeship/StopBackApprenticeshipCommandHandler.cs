using MediatR;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Commands.StopBackApprenticeship
{
    public class StopBackApprenticeshipCommandHandler(ILearnerRepository repository) : IRequestHandler<StopBackApprenticeshipCommand>
    {
        public Task Handle(StopBackApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            return repository.StopBackApprenticeshipId(request, cancellationToken);
        }
    }
}
