using Common.Controls.Validators;
using Microsoft.AspNetCore.Components;

namespace Presentation.Controls;

public class ControlBaseForm : ComponentBase
{
    [Parameter]
    public ControlAttributesModel Model { get; set; } = new();

    public ControlAttributesValidator Validator { get; set; } = new();

    public MudForm Form = new();

    public virtual async Task<bool> IsValid()
    {
        await Form.Validate();
        return Form.IsValid;
    }
}
