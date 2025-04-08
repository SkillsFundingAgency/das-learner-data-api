using MediatR;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.Application.Commands.SaveLearner;

public class SaveLearnerCommandHandler(ILearnerDataRepository repository) : IRequestHandler<SaveLearnerCommand, long>
{
    public async Task<long> Handle(SaveLearnerCommand request, CancellationToken cancellationToken)
    {
        return await repository.Save(request, cancellationToken);
    }
}