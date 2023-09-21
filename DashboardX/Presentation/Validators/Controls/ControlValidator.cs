using FluentValidation;
using Shared.Models.Controls;

namespace Presentation.Validators.Controls;

public class ControlValidator : BaseValidator<Control>
{

    public ControlValidator() : base()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(30);

        RuleFor(x => x.DeviceId)
            .NotEmpty();

        RuleFor(x => x.Icon)
            .NotEmpty();

        RuleFor(x => x.IconBackgroundColor)
            .NotEmpty();

        RuleFor(x => x.Topic)
            .NotEmpty()
            .MaximumLength(64);
    }
}
