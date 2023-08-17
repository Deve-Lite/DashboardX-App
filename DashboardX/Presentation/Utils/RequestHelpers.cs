using Core;
using MudBlazor;

namespace Presentation.Utils;

public class RequestHelpers
{
    public static async Task InvokeAfterRequest(ISnackbar snackbar, IResult result, Func<Task> onSuccess, string sucessMessage = "Action finished successfully.")
    {
        if (result.Succeeded)
        {
            await onSuccess.Invoke();
        }
        else
        {
            foreach (var error in result.Messages)
                snackbar.Add(error, MudBlazor.Severity.Error, config => { config.ShowCloseIcon = false; });
        }
    }
}
