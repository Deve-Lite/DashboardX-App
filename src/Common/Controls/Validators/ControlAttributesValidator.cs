using Common.Controls.Models;

namespace Common.Controls.Validators;

public class ControlAttributesValidator : BaseValidator<ControlAttributesModel>
{
    public ControlAttributesValidator() : base()
    {
        RuleFor(x => x.Payload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Button);

        RuleFor(x => x.PayloadTemplate)
            .Length(1, 256)
            .When(x => x.Type == ControlType.Slider || x.Type == ControlType.DateTime || x.Type == ControlType.Color);

        RuleFor(x => x.MinValue)
            .LessThan(x => x.MaxValue)
            .When(x => x.Type == ControlType.Slider);

        RuleFor(x => x.MaxValue)
             .GreaterThan(x => x.MinValue)
             .When(x => x.Type == ControlType.Slider);

        RuleFor(x => x.OnPayload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Switch || x.Type == ControlType.State);

        RuleFor(x => x.OffPayload)
            .Length(1, 128)
            .When(x => x.Type == ControlType.Switch || x.Type == ControlType.State);
    }
}
