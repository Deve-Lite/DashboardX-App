using Microsoft.AspNetCore.Components;

namespace Presentation.Layout.PlatformLayouts;

public class BaseLayout : MudComponentBase
{
    [Parameter]
    public RenderFragment? Body { get; set; }
}
