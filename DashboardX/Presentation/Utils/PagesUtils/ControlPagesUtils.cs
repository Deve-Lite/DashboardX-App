using Infrastructure;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Presentation.Models;
using Presentation.SharedComponents.Controls;
using Shared.Models.Controls;

namespace Presentation.Utils.PagesUtils;

public static class ControlPagesUtils
{
    public static async Task AddControl(IDialogService dialogService, string DeviceId, Action refreshUI, IStringLocalizer<object> localizer, string ClientId = "")
    {
        var parameters = new DialogParameters<UpsertControlDialog>
        {
            {  x => x.DeviceId, DeviceId },
            {  x => x.ClientId, ClientId }
        };

        var dialog = await dialogService.ShowAsync<UpsertControlDialog>(localizer["Create Control"], parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<Client> ?? Result<Client>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }

    public static async Task UpdateControl(IDialogService dialogService, Control control, string DeviceId, Action refreshUI, IStringLocalizer<object> localizer)
    {
        var parameters = new DialogParameters<UpsertControlDialog>
        {
            {  x => x.DeviceId, control.DeviceId },
            {  x => x.ClientId, DeviceId },
            {  x => x.Control, control }
        };

        var dialog = await dialogService.ShowAsync<UpsertControlDialog>(localizer["Edit Control"], parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result<Client> ?? Result<Client>.Fail(message: localizer["Couldn't parse response."]);

        if (x.Succeeded)
            refreshUI.Invoke();
    }

    public static async Task RemoveControl(IDialogService dialogService, Control control, string DeviceId, IStringLocalizer<object> localizer)
    {
        var parameters = new DialogParameters<RemoveControlDialog>
        {
            {  x => x.DeviceId, control.DeviceId },
            {  x => x.ClientId, DeviceId },
            {  x => x.Control, control }
        };

        var dialog = await dialogService.ShowAsync<RemoveControlDialog>(localizer["Remove Control"], parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        var x = result.Data as Result ?? Result.Fail(message: localizer["Couldn't parse response."]);
    }
}
