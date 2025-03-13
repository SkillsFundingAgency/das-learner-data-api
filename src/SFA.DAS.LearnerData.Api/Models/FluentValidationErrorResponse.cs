using FluentValidation.Results;

namespace SFA.DAS.LearnerData.Api.Models;

public class FluentValidationErrorResponse
{
    public IEnumerable<ValidationFailure> Errors { get; set; }
}