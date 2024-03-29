﻿using Microsoft.AspNetCore.Components;
using Presentation.Shared.Dialogs;
using Presentation.Utils;

namespace Presentation.Controls;

public partial class BaseControl : ComponentBase
{
    [Inject]
    protected IStringLocalizer<BaseControl> BaseLocalizer { get; set; } = default!;
    [Inject]
    protected IDialogService? DialogService { get; set; }
    [Inject]
    protected ISnackbar? SnackbarService { get; set; }

    [Parameter]
    public Control? Control { get; set; }
    [Parameter]
    public IClient? Client { get; set; }
    [Parameter]
    public Device? Device { get; set; }

    [CascadingParameter]
    protected MudTheme? AppTheme { get; set; }

    [CascadingParameter]
    protected bool IsDarkMode { get; set; }

    protected virtual string DefaultIcon => IconUtils.DefualtIcon;

    public virtual string GetIcon()
    {
        if (Control!.SubscribeStatus == ControlSubscribeStatus.FailedToSubscribe)
            return Icons.Material.Filled.SmsFailed;

        return DefaultIcon;
    }

    public virtual string GetBackgroundColor()
    {
        if (Control!.SubscribeStatus == ControlSubscribeStatus.FailedToSubscribe) 
        {

        }

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

    public virtual string GetBorderBackgroundColor()
    {
        if (Control!.SubscribeStatus == ControlSubscribeStatus.FailedToSubscribe)
        {

        }

        if (!Control!.IsAvailable)
        {
            if (IsDarkMode)
                return AppTheme!.PaletteDark.Error.ToString();

            return AppTheme!.Palette.Error.ToString();
        }

        return Control!.Icon.BackgroundHex;
    }

    public virtual string GetName() => string.IsNullOrEmpty(Control?.Name) ? BaseLocalizer!["No name"] : Control.Name;

    public async Task<bool> ConfirmationDialog(string title, string question)
    {
        if (Control!.IsConfiramtionRequired)
        {
            var parameters = new DialogParameters<ConfirmDialog>
            {
                { x => x.Description, question }
            };

            var options = new DialogOptions { NoHeader = true };

            var dialog = await DialogService!.ShowAsync<ConfirmDialog>(title, parameters, options);
            var result = await dialog.Result;

            if (result.Canceled)
                return false;

            bool confirmed = Convert.ToBoolean(result.Data);

            if (!confirmed)
                return false;
        }

        return true;
    }
    public async Task<bool> ConfirmationDialog(string question) => await ConfirmationDialog(BaseLocalizer!["Action Requires Confirmation"], question);

    public async Task<bool> ConfirmationDialog() => await ConfirmationDialog(BaseLocalizer!["Action Requires Confirmation"], BaseLocalizer!["You are required to confirm before sending data."]);

    public async Task PublishMessage(string payload)
    {
        var result = await Client!.PublishAsync(Control!.GetTopic(Device!), payload, Control!.QualityOfService);

        if (!result.Succeeded && result.ShowToast)
            SnackbarService!.Add(result.Messages[0], Severity.Error);
    }
}
