namespace Presentation.Extensions;

public static class JsRuntimeExtensions
{
    private const int MaxMobileScreenWidth = 768;
    private const int MinDesktopScreenWidth = 1440;

    public static async Task<int> GetScreenWidth(this IJSRuntime runtime)
        => await runtime.InvokeAsync<int>("window.getScreenWidth");
    
    public static async Task<bool> IsMobile(this IJSRuntime runtime)
    {
        var width = await runtime.InvokeAsync<int>("window.getScreenWidth");
        return IsMobile(width);
    }

    public static async Task<bool> IsDesktop(this IJSRuntime runtime)
    {
        var width = await runtime.InvokeAsync<int>("window.getScreenWidth");
        return IsDektop(width);
    }

    public static bool IsDektop(int width) => MinDesktopScreenWidth <= width;

    public static bool IsMobile(int width) => width <= MaxMobileScreenWidth;

    public static async Task GoBack(this IJSRuntime runtime)
        => await runtime.InvokeVoidAsync("history.back");

}
