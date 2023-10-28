using Common.Controls.Models;

namespace Common.Controls.Validators;

public class ControlValidator : BaseValidator<Control>
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
            .Length(1, 256)
            .When(x => x.Type == ControlType.Button);

        RuleFor(x => x.Attributes.PayloadTemplate)
            .Length(1, 256)
            .When(x => x.Type == ControlType.Slider);

        RuleFor(x => x.Attributes.OnPayload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Switch);

        RuleFor(x => x.Attributes.OffPayload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Switch);

        RuleFor(x => x.Attributes.NewName)
            .Length(1, 64)
            .When(x => x.Type == ControlType.Radio);

        RuleFor(x => x.Attributes.NewPayload)
            .Length(1, 256)
            .When(x => x.Type == ControlType.Radio);
    }
}
