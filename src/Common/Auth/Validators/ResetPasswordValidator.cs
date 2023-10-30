using Common.Auth.Models;

namespace Common.Auth.Validators;

public class ResetPasswordValidator : BaseValidator<ResetPasswordModel>
{
	public ResetPasswordValidator() : base()
	{
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(30)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords are not matching.");
    }
}

