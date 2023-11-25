using Microsoft.AspNetCore.Components;
using Presentation.Devices.Dialogs;
using System.Text.RegularExpressions;

namespace Presentation.Devices;

public class DevicePagesUtils
{

    public static async Task UpdateDevice(Device device, 
        IDialogService dialogService, 
        Action refreshUI, 
        IStringLocalizer<object> localizer)
    {
        var parameters = new DialogParameters<UpsertDeviceDialog>
        {
            { x => x.Model, device.Dto() }
        };

        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertDeviceDialog>(localizer["Edit Device"], parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<Client> ?? Result<Client>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
        {
            refreshUI.Invoke();
        }
    }

    public static async Task RemoveDevice(Device device, 
        IDialogService dialogService, 
        IStringLocalizer<object> localizer, 
        NavigationManager NavigationManager, 
        IJSRuntime runtime)
    {
        var parameters = new DialogParameters<RemoveDeviceDialog>
        {
            { x => x.Device, device },
            { x => x.ClientId, device.BrokerId }
        };

        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<RemoveDeviceDialog>(localizer["Remove Device"], parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result ?? Result.Fail(message: localizer["Couldn't parse response."]);
        //TODO: Fix 
        if (x.Succeeded)
        {
            var currentPage = NavigationManager.Uri;
            string deviceListPagePattern = @".*/devices$";
            string brokerPagePattern = @".*/brokers/.*";

            if (new Regex(brokerPagePattern).IsMatch(currentPage) || new Regex(deviceListPagePattern).IsMatch(currentPage))
                return;

            await runtime.GoBack();
        }
    }

    public static async Task AddDevice(Action refreshUI,
        IDialogService dialogService,
        IStringLocalizer<object> localizer, 
        string? clientId = null)
    {
        var parameters = new DialogParameters<UpsertDeviceDialog>
        {
            { x => x.Model, new DeviceDTO{ BrokerId = clientId ?? string.Empty } }
        };

        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertDeviceDialog>(localizer["Create Device"], parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<Client> ?? Result<Client>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }
}

