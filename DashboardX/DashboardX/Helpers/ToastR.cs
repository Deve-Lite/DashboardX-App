using Microsoft.JSInterop;

namespace DashboardX.Helpers;

public class ToastR : BaseRuntime
{
    public ToastR(IJSRuntime runtime) : base(runtime)
    {
    }

    public async Task Success(string successMessage)  => await Invoke("Toastr", "success", successMessage);
    public async Task Error(string successMessage) => await Invoke("Toastr", "error", successMessage);
}
