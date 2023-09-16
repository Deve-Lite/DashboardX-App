using FluentValidation;
using Shared.Models.Users;

namespace Presentation.Validators.Auth;

public class ChangePasswordValidator : BaseValidator<ChangePasswordModel>
{
    public ChangePasswordValidator() : base()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(30)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(30)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords are not matching.");
    }
}
