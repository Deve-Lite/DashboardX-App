using Microsoft.JSInterop;

namespace DashboardX.Helpers;

public abstract class BaseRuntime
{
    protected IJSRuntime runtime;
    public BaseRuntime(IJSRuntime runtime)
    {
        this.runtime = runtime;
    }

    public async Task Invoke(string functionName, params object[] args)
        => await runtime.InvokeVoidAsync(functionName, args);

}
