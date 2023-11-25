using Common.Controls.Models;

namespace Common.Controls.Validators;

public class ControlValidator : BaseValidator<ControlDTO>
{

    public ControlValidator() : base()
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

        RuleFor(x => x.Attributes.Payload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Button);

        RuleFor(x => x.Attributes.PayloadTemplate)
            .Length(1, 256)
            .When(x => x.Type == ControlType.Slider || x.Type == ControlType.DateTime || x.Type == ControlType.Color);

        RuleFor(x => x.Attributes.MinValue)
            .LessThan(x => x.Attributes.MaxValue)
            .When(x => x.Type == ControlType.Slider);

        RuleFor(x => x.Attributes.MaxValue)
             .GreaterThan(x => x.Attributes.MinValue)
             .When(x => x.Type == ControlType.Slider);

        RuleFor(x => x.Attributes.OnPayload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Switch || x.Type == ControlType.State);

        RuleFor(x => x.Attributes.OffPayload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Switch || x.Type == ControlType.State);
    }
}
