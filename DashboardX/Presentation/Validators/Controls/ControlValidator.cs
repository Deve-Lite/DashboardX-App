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

        RuleFor(x => x.Attributes.Payload)
            .Length(1, 256)
            .When(x => x.Type == ControlType.Button);

        RuleFor(x => x.Attributes.PayloadTemplate)
            .Length(1, 256)
            .When(x => x.Type == ControlType.Slider);
    }
}
