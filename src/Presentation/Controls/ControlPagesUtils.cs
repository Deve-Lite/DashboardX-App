using Presentation.Controls.Dialogs;

namespace Presentation.Controls;

public static class ControlPagesUtils
{
    public static async Task AddControl(IDialogService dialogService, string DeviceId, string ClientId)
    {
        var parameters = new DialogParameters<UpsertControlDialog>
        {
            {  x => x.Model, new ControlDTO { DeviceId = DeviceId } },
            {  x => x.ClientId, ClientId }
        };
        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertControlDialog>("", parameters, options);
        var result = await dialog.Result;
    }

    public static async Task UpdateControl(IDialogService dialogService, Control control, string DeviceId)
    {
        var parameters = new DialogParameters<UpsertControlDialog>
        {
            {  x => x.ClientId, DeviceId },
            {  x => x.Model, control.Dto() }
        };

        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<UpsertControlDialog>("", parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return;
    }

    public static async Task<bool> RemoveControl(IDialogService dialogService, Control control, string ClientId)
    {
        var parameters = new DialogParameters<RemoveControlDialog>
        {
            {  x => x.DeviceId, control.DeviceId },
            {  x => x.ClientId, ClientId },
            {  x => x.Control, control }
        };
        var options = new DialogOptions()
        {
            NoHeader = true,
        };

        var dialog = await dialogService.ShowAsync<RemoveControlDialog>("", parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
            return false;

        var x = result.Data as Result ?? Result.Fail();

        if (!x.Succeeded)
            return false;

        return true;
    }
}
