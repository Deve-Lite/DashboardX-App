using FluentValidation;
using Shared.Models.Controls;

namespace Presentation.Validators.Controls;

public class ButtonValidator : BaseValidator<Control>
{
    public ButtonValidator() : base()
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

        RuleFor(x => x.Attributes.Payload)
            .NotEmpty()
            .MaximumLength(256);
    }
}
