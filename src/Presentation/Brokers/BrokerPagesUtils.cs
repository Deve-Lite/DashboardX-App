using Microsoft.AspNetCore.Components;
using Presentation.Brokers.Dialogs;
using System.Text.RegularExpressions;

namespace Presentation.Brokers;

public class BrokerPagesUtils
{
    public static async Task UpdateBroker(IClient client, IDialogService dialogService)
    {
        var parameters = new DialogParameters<UpsertBrokerDialog> 
        { 
            { x => x.Model, client.GetBroker().Dto() } 
        };
        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertBrokerDialog>("", parameters, options);
    }

    public static async Task RemoveBroker(IClient client,
                                          IDialogService dialogService,
                                          NavigationManager navigationManager)
    {
        var parameters = new DialogParameters<RemoveBrokerDialog> { { x => x.Broker, client.GetBroker() } };
        var options = new DialogOptions() { NoHeader=true, };

        var dialog = await dialogService.ShowAsync<RemoveBrokerDialog>("", parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result ?? Result.Fail();

        if (x.Succeeded)
        {
            var currentPage = navigationManager.Uri;
            string brokerPagePattern = @".*/brokers/.*";

            if (new Regex(brokerPagePattern).IsMatch(currentPage))
            {
                navigationManager.NavigateTo("/brokers");
            }
        }
    }

    public static async Task AddBroker(IDialogService dialogService)
    {
        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertBrokerDialog>("", options);
    }
}
