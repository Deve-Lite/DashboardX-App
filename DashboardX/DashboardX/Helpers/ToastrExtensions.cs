using Microsoft.JSInterop;

namespace DashboardX.Helpers;

public static class ToastrExtensions
{
    public static async Task Success(this IJSRuntime runtime, string successMessage) 
        => await runtime.InvokeVoidAsync("Toastr", "success", successMessage);

    public static async Task Fail(this IJSRuntime runtime, string successMessage) 
        => await runtime.InvokeVoidAsync("Toastr", "fail", successMessage);
}
