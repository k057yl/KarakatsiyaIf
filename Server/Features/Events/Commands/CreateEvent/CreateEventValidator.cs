using FluentValidation;
using Karakatsiya.Constants;
using System;
using System.Linq;

namespace Karakatsiya.Features.Events.Commands.CreateEvent
{
    public class CreateEventValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventValidator()
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
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(AppConstants.Validation.MAX_CITY_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.Street)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(AppConstants.Validation.MAX_STREET_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.HouseNumber)
                .MaximumLength(AppConstants.Validation.MAX_HOUSE_NUMBER_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.ExternalTicketUrl)
                .MaximumLength(AppConstants.Validation.MAX_URL_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Payload.Photos)
                .Must(photos => photos == null || photos.Count <= 6).WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}