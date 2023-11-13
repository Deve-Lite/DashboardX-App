namespace Presentation.Extensions;

public static class BrowserResizeExtensions
{
    public static event Func<int, Task>? OnResize;

    [JSInvokable]
    public static async Task OnBrowserResize(int width)
    {
       await OnResize?.Invoke(width)!;
    }

    public static async Task SubscribeToResizeEvent(this IJSRuntime _runitme)
    {
       await _runitme.InvokeAsync<object>("browserResize.subscribeToResizeEvent");
    }

    public static async Task UnsubscribeToResizeEvent(this IJSRuntime _runitme)
    {
        await _runitme.InvokeAsync<object>("browserResize.unsubscribeToResizeEvent");
    }
}
