using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Events.Commands.ApproveEvent
{
    public class ApproveEventCommandValidator : AbstractValidator<ApproveEventCommand>
    {
        public ApproveEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.EVENT_ID_REQUIRED);
        }
    }
}
