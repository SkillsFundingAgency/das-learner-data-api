using MediatR;

namespace SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;

public record AssignApprenticeshipIdCommand : IRequest
{
    public long Ukprn { get; set; }
    public long LearnerDataId { get; set; }
    public long? ApprenticeshipId { get; set; }
}