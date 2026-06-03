using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Admin.Commands.VerifyPerformer
{
    public class VerifyPerformerValidator : AbstractValidator<VerifyPerformerCommand>
    {
        public VerifyPerformerValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(200).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(AppConstants.Validation.MAX_URL_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
