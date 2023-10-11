using Common.Auth.Models;

namespace Common.Auth.Validators; 

public class ForgetPasswordValidator : BaseValidator<ForgetPasswordModel>
{
	public ForgetPasswordValidator() : base()
	{
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();
    }
}

