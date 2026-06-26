using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Admin.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(50);

            RuleFor(x => x.Icon)
                .NotEmpty()
                .WithMessage(AppConstants.Errors.VALIDATION_FAILED);
        }
    }
}
