using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Admin.Commands.ApproveOrganizer
{
    public class ApproveOrganizerValidator : AbstractValidator<ApproveOrganizerCommand>
    {
        public ApproveOrganizerValidator()
        {
            RuleFor(x => x.OrganizerId)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
