using MediatR;

namespace SFA.DAS.LearnerData.Application.Commands;

public record CreateLearnerCommand : IRequest
{
    public long Id { get; set; }
}