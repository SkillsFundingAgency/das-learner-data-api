using FluentValidation;

namespace SFA.DAS.LearnerData.Application.Commands;

public class CreateLearnerCommandValidator : AbstractValidator<CreateLearnerCommand>
{
    public CreateLearnerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}