

using DashboardX.Tokens;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DashboardX;

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

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(accessToken.Claims,"jwt"))));
    }

    public void NotifyUserLoggedIn(AccessToken token)
    {
        accessToken = token;

        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(accessToken.Claims, "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        NotifyAuthenticationStateChanged(authState);
    }
}
