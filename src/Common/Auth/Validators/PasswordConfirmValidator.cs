using Common.Users.Models;

namespace Common.Auth.Validators;

public class PasswordConfirmValidator : BaseValidator<PasswordConfirm>
{
    public PasswordConfirmValidator() : base()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(30)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");
    }
}
