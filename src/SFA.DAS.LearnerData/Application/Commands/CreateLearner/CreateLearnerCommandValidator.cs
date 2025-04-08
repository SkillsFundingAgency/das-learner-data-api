using FluentValidation;

namespace SFA.DAS.LearnerData.Application.Commands.CreateLearner;

public class CreateLearnerCommandValidator : AbstractValidator<CreateLearnerCommand>
{
    public CreateLearnerCommandValidator()
    {
        RuleFor(x => x.Ukprn).NotEmpty();
    }
}