namespace Presentation;

public class RequestHelpers
{
    public static async Task InvokeAfterRequest(ISnackbar snackbar, IResult result, Func<Task> onSuccess, bool displayErrors = true)
    {
        if ((Result)result)
        {
            await onSuccess.Invoke();
        }
        else
        {
            if (!displayErrors)
                return;

            foreach (var error in result.Messages)
                snackbar.Add(error, Severity.Error);
        }
    }

    public static void InvokeAfterRequest(ISnackbar snackbar, IResult result, string successMessage = "", bool displayErrors = true)
    {
        if ((Result)result)
        {
            snackbar.Add(successMessage, Severity.Success);
        }
        else
        {
            if (!displayErrors)
                return;

            foreach (var error in result.Messages)
                snackbar.Add(error, Severity.Error);
        }
    }
}
