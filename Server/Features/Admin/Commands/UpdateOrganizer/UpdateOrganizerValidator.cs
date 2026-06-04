using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Admin.Commands.UpdateOrganizer
{
    public class UpdateOrganizerValidator : AbstractValidator<UpdateOrganizerCommand>
    {
        public UpdateOrganizerValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(AppConstants.Validation.MAX_NAME_LENGTH);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(AppConstants.Validation.MAX_EMAIL_LENGTH);

            RuleFor(x => x.Phone)
                .MaximumLength(AppConstants.Validation.MAX_PHONE_LENGTH)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Website)
                .MaximumLength(AppConstants.Validation.MAX_URL_LENGTH)
                .When(x => !string.IsNullOrEmpty(x.Website));

            RuleFor(x => x.Telegram)
                .MaximumLength(AppConstants.Validation.MAX_SOCIAL_LENGTH)
                .When(x => !string.IsNullOrEmpty(x.Telegram));

            RuleFor(x => x.Instagram)
                .MaximumLength(AppConstants.Validation.MAX_SOCIAL_LENGTH)
                .When(x => !string.IsNullOrEmpty(x.Instagram));
        }
    }
}
