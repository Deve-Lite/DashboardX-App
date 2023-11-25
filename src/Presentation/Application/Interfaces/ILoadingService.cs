namespace Presentation.Application.Interfaces;

public interface ILoadingService
{
    bool IsLoading { get; }
    bool IsDialogLoading { get; }

    Task<IResult> InvokeAsync(Func<Task<IResult>> action);
    Task<IResult> InvokeDialogAsync(Func<Task<IResult>> action);
    void SetRefreshAction(Func<Task> refreshAction);
    void RemoveRefreshAction();
}
