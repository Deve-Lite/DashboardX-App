using Common.Controls.Models;

namespace Common.Controls.Validators;

public class ControlModelValidator : BaseValidator<ControlModel>
{

    public ControlModelValidator() : base()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(30);

        RuleFor(x => x.DeviceId)
            .NotEmpty();

        RuleFor(x => x.Icon.Name)
            .NotEmpty();

        RuleFor(x => x.Icon.BackgroundHex)
            .NotEmpty();

        RuleFor(x => x.Topic)
            .NotEmpty()
            .MaximumLength(64);
    }
}
