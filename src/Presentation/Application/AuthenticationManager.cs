using Microsoft.AspNetCore.Components;
using Presentation.Application.Interfaces;

namespace Presentation.Application;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly IClientService _clientService;
    private readonly IPrefrenceService _preferenceService;
    private readonly ApplicationStateProvider _applicationStateProvider;
    private readonly NavigationManager _navigationManager;

    public AuthenticationManager(AuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager)
    {

        //_clientService = clientService;
        //_preferenceService = preferenceService;
        _applicationStateProvider = (ApplicationStateProvider)authenticationStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task ExtendSession(string accessToken, string refreshToken)
    {
        await _applicationStateProvider.ExtendSession(accessToken, refreshToken);
    }

    public string GetRefreshToken()
    {
        return _applicationStateProvider.RefreshToken;
    }

    public async Task Logout()
    {
        //await _clientService.Logout();
        //await _preferenceService.RestroreDefaultPreferences();
        await _applicationStateProvider.Logout();
        _navigationManager.NavigateTo("/auth/login");
    }
}
