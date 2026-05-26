using FluentValidation;
using Karakatsiya.Constants;

namespace Karakatsiya.Features.Comments.Commands.CreateComment
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage(AppConstants.Errors.VALIDATION_FAILED)
                .MaximumLength(AppConstants.Validation.MAX_COMMENT_LENGTH).WithMessage(AppConstants.Errors.VALIDATION_FAILED);

            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage(AppConstants.Errors.EVENT_ID_REQUIRED);
        }
    }
}
