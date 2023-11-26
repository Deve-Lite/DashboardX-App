using Microsoft.AspNetCore.Components;
using Presentation.Devices.Dialogs;
using System.Text.RegularExpressions;

namespace Presentation.Devices;

public class DevicePagesUtils
{
    public static async Task UpdateDevice(Device device, IDialogService dialogService)
    {
        var parameters = new DialogParameters<UpsertDeviceDialog>
        {
            { x => x.Model, device.Dto() }
        };

        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertDeviceDialog>("", parameters, options);
    }

    public static async Task RemoveDevice(Device device, IDialogService dialogService, NavigationManager NavigationManager)
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

        var dialog = await dialogService.ShowAsync<RemoveDeviceDialog>("", parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result ?? Result.Fail();

        if (x.Succeeded)
        {
            var currentPage = NavigationManager.Uri;
            string deviceListPagePattern = @".*/devices$";
            string brokerPagePattern = @".*/brokers/.*";

            if (new Regex(brokerPagePattern).IsMatch(currentPage) || new Regex(deviceListPagePattern).IsMatch(currentPage))
                return;

            NavigationManager.NavigateTo("/devices");
        }
    }

    public static async Task AddDevice(IDialogService dialogService, string? clientId = null)
    {
        var parameters = new DialogParameters<UpsertDeviceDialog>
        {
            { x => x.Model, new DeviceDTO{ BrokerId = clientId ?? string.Empty } }
        };

        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertDeviceDialog>("", parameters, options);
    }
}

