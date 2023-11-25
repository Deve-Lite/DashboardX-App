using Microsoft.AspNetCore.Components;
using Presentation.Brokers.Dialogs;
using System.Text.RegularExpressions;

namespace Presentation.Brokers;

public class BrokerPagesUtils
{
    public static async Task UpdateBroker(IClient client,
                                          IDialogService dialogService,
                                          Action refreshUI,
                                          IStringLocalizer<object> localizer)
    {
        var parameters = new DialogParameters<UpsertBrokerDialog> { { x => x.Model, client.GetBroker().Dto() } };
        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertBrokerDialog>(localizer["Edit Broker"], parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<Client> ?? Result<Client>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }

    public static async Task RemoveBroker(IClient client,
                                          IDialogService dialogService,
                                          Action refreshUI,
                                          IStringLocalizer<object> localizer,
                                          NavigationManager navigationManager,
                                          IJSRuntime runtime)
    {
        var parameters = new DialogParameters<RemoveBrokerDialog> { { x => x.Broker, client.GetBroker() } };
        var options = new DialogOptions() { NoHeader=true, };

        var dialog = await dialogService.ShowAsync<RemoveBrokerDialog>("", parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result ?? Result.Fail(message: localizer["Couldn't parse response."]);
        //TODO: Fix 

        if (x.Succeeded)
        {
            var currentPage = navigationManager.Uri;
            string brokerPagePattern = @".*/brokers/.*";

            if (new Regex(brokerPagePattern).IsMatch(currentPage))
            {
                //await runtime.GoBack();
            }
            else
            {
                refreshUI.Invoke();
            }
        }
    }

    public static async Task AddBroker(IDialogService dialogService,
                                       Action refreshUI,
                                       IStringLocalizer<object> localizer)
    {
        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertBrokerDialog>(localizer["Create Broker"], options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<IClient> ?? Result<IClient>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }
}
