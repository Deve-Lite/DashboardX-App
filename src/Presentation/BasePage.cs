using Microsoft.AspNetCore.Components;

namespace Presentation;

public class BasePage : ComponentBase
{
    public bool IsMobile { get; set; }
    public bool LoadedSuccessfully { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        //TODO: Uncoment = await _runtime.IsMobile();
        IsMobile = false;

        await base.OnInitializedAsync();
    }
}
