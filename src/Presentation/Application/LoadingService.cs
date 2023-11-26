using Presentation.Application.Interfaces;

namespace Presentation.Application;

public class LoadingService : ILoadingService
{
    private bool isLoadingDialog;
    private bool isLoading;
    private Func<Task> refreshTask;

    public bool IsLoading => isLoading;
    public bool IsDialogLoading => isLoadingDialog;

    public LoadingService()
    {
        isLoading = false;
        isLoadingDialog = false;
        refreshTask = default!;
    }

    public async Task<IResult> InvokeAsync(Func<Task<IResult>> action)
    {
        if(isLoading || isLoadingDialog)  
            return Result.Fail(message: "Loading in progress");
        
        try
        {
            isLoading = true;
            //await refreshTask?.Invoke()!;

            var result = await action.Invoke();

            isLoading = false;
            await refreshTask?.Invoke()!;

            return result;
        }
        catch (Exception ex)
        {
            //TODO: Locliazer
            return Result.Fail(message: $"Unknown exception occured {nameof(ex)}");
        }
    }

    public async Task<IResult> InvokeDialogAsync(Func<Task<IResult>> action)
    {
        if (isLoading || isLoadingDialog)
            return Result.Fail(message: "Loading in progress");

        try
        {
            isLoadingDialog = true;
            //await refreshTask?.Invoke()!;

            var result = await action.Invoke();

            isLoadingDialog = false;
            await refreshTask?.Invoke()!;

            return result;
        }
        catch (Exception ex)
        {
            //TODO: Locliazer
            return Result.Fail(message: $"Unknown exception occured {nameof(ex)}");
        }
    }

    public void SetRefreshAction(Func<Task> refreshAction) 
    {
        refreshTask = refreshAction;
    }

    public void RemoveRefreshAction()
    {
        refreshTask = default!;
    }

    public void ShowLoading()
    {
        isLoading = true;
    }

    public void HideLoading()
    {
        isLoading = false;
    }
}
