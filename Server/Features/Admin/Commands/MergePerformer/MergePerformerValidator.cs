using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Admin.Commands.MergePerformer
{
    public class MergePerformerValidator : AbstractValidator<MergePerformerCommand>
    {
        public MergePerformerValidator()
        {
            RuleFor(x => x.SourceId)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.TargetId)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x)
                .Must(x => x.SourceId != x.TargetId).WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
