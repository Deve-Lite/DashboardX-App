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

    [CascadingParameter]
    protected MudTheme? AppTheme { get; set; }

    [CascadingParameter]
    private bool IsDarkMode { get; set; }

    public string GetBackgroundColor()
    {
        if (!Control!.IsAvailable)
        {
            var background = AppTheme!.Palette.DrawerBackground;
            if (IsDarkMode)
                background = AppTheme!.PaletteDark.DrawerBackground;

            background.SetAlpha(background.A * 0.5);
            return background.ToString();
        }

        return $"{Control!.Icon.BackgroundHex}80";
    }

    public string GetBorderBackgroundColor()
    {
        if (!Control!.IsAvailable)
        {
            if (IsDarkMode)
                return AppTheme!.PaletteDark.Error.ToString();

            return AppTheme!.Palette.Error.ToString();
        }

        return Control!.Icon.BackgroundHex;
    }

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
