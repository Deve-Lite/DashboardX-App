using Microsoft.AspNetCore.Components;
using Presentation.Shared;

namespace Presentation.Controls;

public partial class BaseControl : ComponentBase
{
    [Inject]
    protected IStringLocalizer<BaseControl>? Localizer { get; set; }

    [Inject]
    protected IDialogService? DialogService { get; set; }

    [Parameter]
    public Control? Control { get; set; }
    [Parameter]
    public Client? Client { get; set; }
    [Parameter]
    public Device? Device { get; set; }

    public string GetBackgroundColor() => $"{Control!.Icon.BackgroundHex}80";

    public string GetName() => string.IsNullOrEmpty(Control?.Name) ? Localizer!["No name"] : Control.Name;

    public async Task<bool> ConfirmationDialog() 
    {
        if (Control!.IsConfiramtionRequired)
        {
            var parameters = new DialogParameters<ConfirmDialog>
            {
                { x => x.Description, Localizer!["You are required to confirm before sending data."] }
            };
            var dialog = await DialogService!.ShowAsync<ConfirmDialog>(Localizer["Confirmation"]!, parameters);
            var result = await dialog.Result;

            if (result.Canceled)
                return false;

            bool confirmed = Convert.ToBoolean(result.Data);

            if (!confirmed)
                return false;
        }

        return true;
    }

}
