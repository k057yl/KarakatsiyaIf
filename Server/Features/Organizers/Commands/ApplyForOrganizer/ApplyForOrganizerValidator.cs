using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Organizers.Commands.ApplyForOrganizer
{
    public class ApplyForOrganizerValidator : AbstractValidator<ApplyForOrganizerCommand>
    {
        public ApplyForOrganizerValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(AppConstants.Errors.INVALID_EMAIL)
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x)
                .Must(HaveAtLeastOneContact)
                .WithMessage(AppConstants.Errors.NO_CONTACTS_PROVIDED);
        }

        private bool HaveAtLeastOneContact(ApplyForOrganizerCommand cmd)
        {
            return !string.IsNullOrWhiteSpace(cmd.Phone) ||
                   !string.IsNullOrWhiteSpace(cmd.Email) ||
                   !string.IsNullOrWhiteSpace(cmd.Telegram);
        }
    }
}
