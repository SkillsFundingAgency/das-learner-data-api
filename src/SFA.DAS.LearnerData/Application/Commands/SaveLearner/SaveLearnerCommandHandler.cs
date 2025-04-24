using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

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

public class SaveLearnerCommandHandler(ILearnerRepository repository) : IRequestHandler<SaveLearnerCommand, SaveLearnerCommandResponse>
{
    public async Task<SaveLearnerCommandResponse> Handle(SaveLearnerCommand request, CancellationToken cancellationToken)
    {
        return await repository.Save(request, cancellationToken);
    }
}