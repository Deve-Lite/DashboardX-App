using Microsoft.AspNetCore.Components;
using Presentation.Application.Interfaces;

namespace Presentation.Application;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly IPrefrenceService _preferenceService;
    private readonly ApplicationStateProvider _applicationStateProvider;
    private readonly NavigationManager _navigationManager;

    private List<ILogoutObserver> _logoutObservers;

    public AuthenticationManager(AuthenticationStateProvider authenticationStateProvider,
                                 NavigationManager navigationManager,
                                 IPrefrenceService prefrenceService)
    {
        _applicationStateProvider = (ApplicationStateProvider)authenticationStateProvider;
        _navigationManager = navigationManager;
        _preferenceService = prefrenceService;
        _logoutObservers = new List<ILogoutObserver>();
    }

    public async Task ExtendSession(string accessToken, string refreshToken)
    {
        await _applicationStateProvider.ExtendSession(accessToken, refreshToken);
    }

    public string GetRefreshToken()
    {
        return _applicationStateProvider.RefreshToken;
    }

    public void ObserveLogout(ILogoutObserver logoutObserver)
    {
        _logoutObservers.Add(logoutObserver);
    }

    public async Task Logout()
    {
        foreach(var observer in _logoutObservers)
            await observer.Logout();

        await _preferenceService.RestroreDefaultPreferences();
        await _applicationStateProvider.Logout();
        _navigationManager.NavigateTo("/auth/login");
    }
}
