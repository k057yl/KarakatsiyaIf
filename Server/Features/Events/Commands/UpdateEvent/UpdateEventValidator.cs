using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Events.Commands.UpdateEvent
{
    public class UpdateEventValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventValidator()
        {
            RuleFor(x => x.Payload.Title)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(AppConstants.General.MAX_TITLE_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.Description)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.StartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage(AppConstants.Errors.INVALID_DATE);

            RuleFor(x => x.Payload.LocationName)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.City)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.Street)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
