using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Events.Commands.RejectEvent
{
    public class RejectEventCommandValidator : AbstractValidator<RejectEventCommand>
    {
        public RejectEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.EVENT_ID_REQUIRED);

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.REASON_REQUIRED)
                .MinimumLength(AppConstants.Validation.MIN_REASON_LENGTH)
                .WithMessage(AppConstants.Errors.REASON_TOO_SHORT)
                .MaximumLength(AppConstants.Validation.MAX_REASON_LENGTH)
                .WithMessage(AppConstants.Errors.REASON_TOO_LONG);
        }
    }
}
