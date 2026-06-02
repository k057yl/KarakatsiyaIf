using FluentValidation;
using Karakatsiya.Constants;
using System;

namespace Karakatsiya.Features.Events.Commands.UpdateEvent
{
    public class UpdateEventValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(AppConstants.General.MAX_TITLE_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage(AppConstants.Errors.INVALID_DATE);

            RuleFor(x => x.LocationName)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}