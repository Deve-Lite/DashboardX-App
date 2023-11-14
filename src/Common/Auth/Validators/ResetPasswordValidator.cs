using Common.Auth.Models;

namespace Common.Auth.Validators;

public class ResetPasswordValidator : BaseValidator<ResetPasswordModel>
{
	public ResetPasswordValidator() : base()
	{
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

