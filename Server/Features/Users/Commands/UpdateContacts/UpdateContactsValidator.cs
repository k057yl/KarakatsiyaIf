using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Users.Commands.UpdateContacts
{
    public class UpdateContactsValidator : AbstractValidator<UpdateContactsCommand>
    {
        public UpdateContactsValidator()
        {
            RuleFor(x => x.Phone)
                .MaximumLength(AppConstants.Validation.MAX_PHONE_LENGTH)
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Website)
                .MaximumLength(AppConstants.Validation.MAX_URL_LENGTH)
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Telegram)
                .MaximumLength(AppConstants.Validation.MAX_SOCIAL_LENGTH)
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Instagram)
                .MaximumLength(AppConstants.Validation.MAX_SOCIAL_LENGTH)
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
