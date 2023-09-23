namespace Presentation.Application;

public interface ILoadingService
{
    Func<bool, Task> OnLoadingChanged { get; set; }
    bool IsLoading { get; }
    void ShowLoading();
    void HideLoading();
}
