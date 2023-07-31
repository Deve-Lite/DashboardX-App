using Microsoft.JSInterop;
using Presentation.Services.Interfaces;

namespace Presentation.Services;

public class ToastService : IToastService
{
    protected readonly IJSRuntime _ijsRuntime;
    public ToastService(IJSRuntime runtime)
    {
        _ijsRuntime = runtime;
    }

    public async Task Success(string successMessage) 
        => await _ijsRuntime.InvokeVoidAsync("Toastr", "success", successMessage);

    public async Task Error(string errorMessage)
        => await _ijsRuntime.InvokeVoidAsync("Toastr", "success", errorMessage);

    public async Task Warning(string warningMessage)
        => await _ijsRuntime.InvokeVoidAsync("Toastr", "success", warningMessage);

    public async Task Info(string message)
        => await _ijsRuntime.InvokeVoidAsync("Toastr", "success", message);
}
