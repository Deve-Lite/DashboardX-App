namespace Presentation.Extensions;

public static class JsRuntimeExtensions
{
    private const int MaxMobileScreenWidth = 768;
    private const int MinDesktopScreenWidth = 1440;

    public static async Task<bool> IsMobile(this IJSRuntime runtime)
    {
        var width = await runtime.InvokeAsync<int>("window.getScreenWidth", MaxMobileScreenWidth);
        return width <= MaxMobileScreenWidth;
    }

    public static async Task<bool> IsDesktop(this IJSRuntime runtime)
    {
        var width = await runtime.InvokeAsync<int>("window.getScreenWidth", MaxMobileScreenWidth);
        return MinDesktopScreenWidth <= width;
    }

    public static async Task GoBack(this IJSRuntime runtime)
        => await runtime.InvokeVoidAsync("history.back");

}
