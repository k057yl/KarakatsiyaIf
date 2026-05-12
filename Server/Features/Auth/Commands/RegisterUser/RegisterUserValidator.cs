using FluentValidation;

namespace Karakatsiya.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .When(x => !string.IsNullOrEmpty(x.Password));
        }
    }
}
