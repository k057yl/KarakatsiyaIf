using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Admin.Commands.RejectOrganizer
{
    public class RejectOrganizerValidator : AbstractValidator<RejectOrganizerCommand>
    {
        public RejectOrganizerValidator()
        {
            RuleFor(x => x.OrganizerId)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(500)
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
