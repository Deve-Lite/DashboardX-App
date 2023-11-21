using Microsoft.AspNetCore.Components;

namespace Presentation.Layout.Navigation;

public class BaseNavigation : MudComponentBase
{
    [Parameter]
    public RenderFragment? Body { get; set; }
}
