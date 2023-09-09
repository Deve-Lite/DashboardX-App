using FluentValidation;
using Shared.Models.Auth;

namespace Presentation.Validators.Auth;

public class LoginValidator : BaseValidator<LoginModel>
{
    public LoginValidator() : base()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(30);
    }
}
