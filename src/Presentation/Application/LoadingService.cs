namespace Presentation.Application;

public class LoadingService : ILoadingService
{
    public Func<bool, Task> OnLoadingChanged { get; set; }

    private bool isLoading = false;

    public LoadingService()
    {
        isLoading = false;
        OnLoadingChanged = default!;
    }

    public bool IsLoading => isLoading;
    public void ShowLoading()
    {
        isLoading = true;
        OnLoadingChanged?.Invoke(IsLoading);
    }

    public void HideLoading()
    {
        isLoading = false;
        OnLoadingChanged?.Invoke(IsLoading);
    }

}
