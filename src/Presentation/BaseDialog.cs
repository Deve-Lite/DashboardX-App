using Microsoft.AspNetCore.Components;

namespace Presentation;

public class BaseDialog : MudDialog, IBrowserViewportObserver, IAsyncDisposable
{
    [Inject]
    public IBrowserViewportService? BrowserViewportService { get; set; }

    [CascadingParameter]
    public MudDialogInstance? Dialog { get; set; }

    public Guid Id { get; } = Guid.NewGuid();
    public ResizeOptions ResizeOptions { get; } = new() { NotifyOnBreakpointOnly = false };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && IsBrowserViewportsEventsAvailable())
        {
            await BrowserViewportService!.SubscribeAsync(this, true);
            await UpdateDialogOptions( await BrowserViewportService.GetCurrentBreakpointAsync());
            await InvokeAsync(StateHasChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public void Cancel() => Dialog!.Cancel();

    public async ValueTask DisposeAsync()
    {
        if(IsBrowserViewportsEventsAvailable()) 
            await BrowserViewportService?.UnsubscribeAsync(this)!;
    }
    public async Task NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        await UpdateDialogOptions(browserViewportEventArgs.Breakpoint);
    }

    protected virtual bool IsBrowserViewportsEventsAvailable() => true;

    protected virtual async Task UpdateDialogOptions(Breakpoint breakpoint)
    {
        var options = Dialog!.Options;

        switch (breakpoint)
        {
            case Breakpoint.Xs:
                options.FullScreen = true;
                break;
            default:
                options.FullScreen = false;
                break;
        }

        Dialog.SetOptions(options);
        await InvokeAsync(StateHasChanged);
    }
}
