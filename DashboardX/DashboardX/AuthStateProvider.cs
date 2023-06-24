
using Blazored.LocalStorage;
using DashboardX.Tokens;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace DashboardX;

//TODO: Implement this class
public class AuthStateProvider : AuthenticationStateProvider
{
    private AccessToken accessToken;

    public AuthStateProvider()
    {
        accessToken = new AccessToken();
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!accessToken.IsValid)
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        //TODO: Read from token
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    public void NotifyUserLoggedIn(AccessToken token)
    {
        accessToken = token;
        //TODO: Read from token
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        NotifyAuthenticationStateChanged(authState);
    }
}
