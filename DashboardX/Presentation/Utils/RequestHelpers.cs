using Core;
using Infrastructure;
using MudBlazor;

namespace Presentation.Utils;

public class RequestHelpers
{
    public static async Task InvokeAfterRequest(ISnackbar snackbar, IResult result, Func<Task> onSuccess, bool displayErrors = true)
    {
        if ((Result) result)
        {
            await onSuccess.Invoke();
        }
        else
        {
            if (!displayErrors)
                return;

            foreach (var error in result.Messages)
                snackbar.Add(error, MudBlazor.Severity.Error, config => { config.ShowCloseIcon = false; });
        }
    }
}
