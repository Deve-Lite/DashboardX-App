namespace Presentation.Utils;

public class RequestHelpers
{
    public static async Task<bool> InvokeAfterRequest(ISnackbar snackbar, IResult result, Func<Task> onSuccess, bool displayErrors = true)
    {
        if ((Result)result)
        {
            await onSuccess.Invoke();
            return true;
        }

        if (displayErrors)
            foreach (var error in result.Messages)
                snackbar.Add(error, Severity.Error);

        return false;
    }

    public static bool InvokeAfterRequest(ISnackbar snackbar, IResult result, Action onSuccess, bool displayErrors = true)
    {
        if ((Result)result)
        {
            onSuccess.Invoke();
            return true;
        }

        if (displayErrors)
            foreach (var error in result.Messages)
                snackbar.Add(error, Severity.Error);

        return false;
    }

    public static bool InvokeAfterRequest(ISnackbar snackbar, IResult result, string successMessage = "", bool displayErrors = true)
    {
        if ((Result)result)
        {
            snackbar.Add(successMessage, Severity.Success);
            return true;
        }

        if (displayErrors)
            foreach (var error in result.Messages)
                snackbar.Add(error, Severity.Error);

        return false;
    }
}
