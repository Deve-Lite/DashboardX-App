using Microsoft.JSInterop;

namespace Presentation.Utils;

public static class JsRuntimeExtensions
{
    private const int MaxMobileScreenWidth = 768;
  
    public static async Task<bool> IsMobile(this IJSRuntime runtime)
        => await runtime.InvokeAsync<bool>("window.isMobile", MaxMobileScreenWidth);
  
    public static async Task GoBack(this IJSRuntime runtime) 
        => await runtime.InvokeVoidAsync("history.back");

}
