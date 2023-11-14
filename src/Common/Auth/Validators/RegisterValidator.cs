using Common.Auth.Models;
using FluentValidation;

namespace Common.Auth.Validators;

public class RegisterValidator : BaseValidator<RegisterModel>
{
    public RegisterValidator() : base()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithMessage("Password should have at least 6 signs.")
            .MaximumLength(30)
            .WithMessage("Password cannot exceed 30 signs.")
            .Must(x => x.Any(char.IsDigit))
            .WithMessage("Password must contain digit")
            .Must(x => x.Any(char.IsUpper))
            .WithMessage("Password must contain upper letter,")
            .Must(x => x.Any(char.IsLower))
            .WithMessage("Password must contain lower letter.");


        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords are not matching.");
    }
}
