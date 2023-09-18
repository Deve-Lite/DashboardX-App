using Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Presentation.Models;
using Presentation.Shared.Devices;
using Shared.Models.Devices;

namespace Presentation.Utils.PagesUtils;

public class DevicePagesUtils
{

    public static async Task UpdateDevice(Device device, 
        IDialogService dialogService, 
        Action refreshUI, 
        IStringLocalizer<object> localizer)
    {
        var parameters = new DialogParameters<UpsertDeviceDialog>
        {
            { x => x.Device, device },
            { x => x.ClientId, device.BrokerId }
        };

        var dialog = await dialogService.ShowAsync<UpsertDeviceDialog>(localizer["Edit Device"], parameters);
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
        NavigationManager _navigationManager)
    {
        var parameters = new DialogParameters<RemoveDeviceDialog>
        {
            { x => x.Device, device },
            { x => x.ClientId, device.BrokerId }
        };

        var dialog = await dialogService.ShowAsync<RemoveDeviceDialog>(localizer["Remove Device"], parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result ?? Result.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
        {
            _navigationManager.NavigateTo("/devices");
        }
    }

    public static async Task AddDevice(Action refreshUI,
        IDialogService dialogService,
        IStringLocalizer<object> localizer, string? clientId = null)
    {
        var parameters = new DialogParameters<RemoveDeviceDialog>
        {
            { x => x.ClientId, clientId }
        };

        var dialog = await dialogService.ShowAsync<UpsertDeviceDialog>(localizer["Create Device"]);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<Client> ?? Result<Client>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }
}

