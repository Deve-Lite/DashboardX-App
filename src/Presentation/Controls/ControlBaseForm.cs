using Common.Controls.Validators;
using Microsoft.AspNetCore.Components;

namespace Presentation.Controls;

public class ControlBaseForm : ComponentBase
{
    [Parameter]
    public ControlAttributesModel Model { get; set; } = new();

    [Inject]
    public IStringLocalizer<ControlAttributesModelValidator> _localizer { get; set; }

    public ControlAttributesModelValidator Validator { get; set; }

    public MudForm Form = new();

    protected override void OnInitialized()
    {
        Validator = new ControlAttributesModelValidator(_localizer);
    }

    public virtual async Task<bool> IsValid()
    {
        await Form.Validate();
        return Form.IsValid;
    }
}
