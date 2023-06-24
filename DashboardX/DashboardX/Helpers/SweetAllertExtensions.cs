using Microsoft.JSInterop;

namespace DashboardX.Helpers;

public static class SweetAllertExtensions
{
    public static async Task Success(this IJSRuntime runtime, string successMessage="Operation finished successfully.") 
    {
        await runtime.InvokeVoidAsync("ShowAllert", "success", successMessage);
    }

    public static async Task Error(this IJSRuntime runtime, string successMessage = "Operation failed.")
    {
        await runtime.InvokeVoidAsync("ShowAllert", "error", successMessage);
    }

    public static async Task Unauthorised(this IJSRuntime runtime, string successMessage = "Operation failed. Logging out user.")
    {
        await runtime.InvokeVoidAsync("ShowAllert", "error", successMessage);
    }
}
