using Microsoft.AspNetCore.Components;

namespace Presentation;

public class BasePage : ComponentBase
{
    [Inject]
    protected ILogger<BasePage> Logger { get; set; } = default!;
    protected bool LoadedSuccessfully { get; set; } = true;
    protected List<BreadcrumbItem> BreadcrumbItems { get; set; } = new();

    private bool awaitingRefresh = false;

    protected Task RerenderPage()
    {
        if (awaitingRefresh)
        {
            Logger.LogInformation("UI refresh skipped due to existing refresh scheduled.");
            return Task.CompletedTask;
        }

        Logger.LogInformation("UI refresh scheduled.");

        awaitingRefresh = true;

        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += async (sender, e) =>
        {
            await InvokeAsync(StateHasChanged);
            awaitingRefresh = false;
        };

        timer.Start();

        return Task.CompletedTask;
    }
}
