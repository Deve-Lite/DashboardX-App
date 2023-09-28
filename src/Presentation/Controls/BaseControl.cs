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

    protected string BackgroundColor { get; set; } = string.Empty;
    protected string Icon { get; set; } = string.Empty;
    protected string Name { get; set; } = string.Empty;

    protected override Task OnInitializedAsync()
    {

        BackgroundColor = $"{Control!.Icon.BackgroundHex}AA";
        Icon = IconUtils.IconList.GetValueOrDefault(Control.Icon.Name, "");
        Name = string.IsNullOrEmpty(Control.Name) ? Localizer!["No name"] : Control.Name;

        //TODO: If not available then change color to gray
        return base.OnInitializedAsync();
    }


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
