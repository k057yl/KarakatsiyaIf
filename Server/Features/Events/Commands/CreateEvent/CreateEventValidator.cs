using FluentValidation;
using Karakatsiya.Constants;
using System;

namespace Karakatsiya.Features.Events.Commands.CreateEvent
{
    public class CreateEventValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventValidator()
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
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(AppConstants.Validation.MAX_CITY_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(AppConstants.Validation.MAX_STREET_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.HouseNumber)
                .MaximumLength(AppConstants.Validation.MAX_HOUSE_NUMBER_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.ExternalTicketUrl)
                .MaximumLength(AppConstants.Validation.MAX_URL_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Photos)
                .Must(photos => photos == null || photos.Count <= 6).WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}