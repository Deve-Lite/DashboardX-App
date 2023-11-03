using Presentation.Brokers.Dialogs;

namespace Presentation.Brokers;

public class BrokerPagesUtils
{
    public static async Task UpdateBroker(IClient client, IDialogService dialogService, Action refreshUI, IStringLocalizer<object> localizer)
    {
        var parameters = new DialogParameters<UpsertBrokerDialog> { { x => x.Model, client.Broker.Dto() } };

        var dialog = await dialogService.ShowAsync<UpsertBrokerDialog>(localizer["Edit Broker"], parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<Client> ?? Result<Client>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }

    public static async Task RemoveBroker(IClient client, IDialogService dialogService, Action refreshUI, IStringLocalizer<object> localizer)
    {
        var parameters = new DialogParameters<RemoveBrokerDialog> { { x => x.Broker, client.Broker } };

        var dialog = await dialogService.ShowAsync<RemoveBrokerDialog>(localizer["Remove Broker"], parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result ?? Result.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }

    public static async Task AddBroker(IDialogService dialogService, Action refreshUI, IStringLocalizer<object> localizer)
    {
        var dialog = await dialogService.ShowAsync<UpsertBrokerDialog>(localizer["Create Broker"]);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<IClient> ?? Result<IClient>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }
}
