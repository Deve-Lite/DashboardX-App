using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Presentation.Application.Interfaces;
using Presentation.Utils;

namespace Presentation.Auth;

public class BaseAuthPage : BasePage
{
    [Inject]
    protected IAuthenticationService AuthenticationService { get; set; } = default!;
    [Inject]
    protected ILoadingService LoadingService { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected IUserService UserService { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IStringLocalizer<BaseAuthPage> AuthLocalizer { get; set; } = default!;
    [Inject]
    protected ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject]
    protected ISessionStorageService SessionStorage { get; set; } = default!;
    [Inject]
    protected AuthenticationStateProvider AuthenticationState { get; set; } = default!;

    protected MudForm Form = new();
    protected bool IsDebug { get; set; }
    protected bool RememberMe { get; set; }

    protected override async Task OnInitializedAsync()
    {
    #if DEBUG
        IsDebug = true;
    #endif
        await base.OnInitializedAsync();

        var result = await LoadingService.InvokeAsync(CheckAuthenticationState);

        if (result.Succeeded)
            await OnSuccessfullLogin();
    }

    protected async Task OnSuccessfullLogin()
    {
        var result = await UserService.GetUser();

        if (!result.Succeeded)
            Snackbar.Add(AuthLocalizer["Couldn't load user settings."], Severity.Warning);

        Snackbar.Add(AuthLocalizer["Hello there!"], Severity.Success);

        NavigationManager.NavigateTo("/brokers");
    }

    protected async Task<IResult> CheckAuthenticationState()
    {
        RememberMe = await LocalStorage.GetItemAsync<bool>(AuthConstraints.RememberMeName);

        var refreshToken = await SessionStorage.GetItemAsync<string>(AuthConstraints.RefreshToken);

        if (string.IsNullOrEmpty(refreshToken) && RememberMe)
            refreshToken = await LocalStorage.GetItemAsync<string>(AuthConstraints.RefreshToken);

        if (string.IsNullOrEmpty(refreshToken))
            return Result.Fail();

        var resp = await AuthenticationService.ReAuthenticate(refreshToken);

        return resp;
    }
}

