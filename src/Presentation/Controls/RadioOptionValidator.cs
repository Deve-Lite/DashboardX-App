using FluentValidation;

namespace Presentation.Controls;

public class RadioOptionValidator : BaseValidator<RadioOption>
{
    public RadioOptionValidator() : base()
    {

        RuleFor(x => x.Name)
            .Length(1, 64);

        RuleFor(x => x.Payload)
            .Length(1, 256);
    }
}
