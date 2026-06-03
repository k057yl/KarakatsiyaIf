using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Performers.Commands.CreatePerformer
{
    public class CreatePerformerValidator : AbstractValidator<CreatePerformerCommand>
    {
        public CreatePerformerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(200).WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
