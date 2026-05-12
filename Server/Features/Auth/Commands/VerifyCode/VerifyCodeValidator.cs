using FluentValidation;

namespace Karakatsiya.Features.Auth.Commands.VerifyCode
{
    public class VerifyCodeValidator : AbstractValidator<VerifyCodeCommand>
    {
        public VerifyCodeValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Code).NotEmpty().Length(6);
        }
    }
}
