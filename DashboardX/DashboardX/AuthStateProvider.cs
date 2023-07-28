

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
            return Task.FromResult(NoAuthState());

        return Task.FromResult(AuthState());
    }

    public void NotifyUserLoggedIn(AccessToken token)
    {
        accessToken = token;
        NotifyAuthenticationStateChanged(Task.FromResult(AuthState()));
    }

    public void NotifyUserLogout() => NotifyAuthenticationStateChanged(Task.FromResult(NoAuthState()));

    private AuthenticationState NoAuthState() => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    private AuthenticationState AuthState()
    {
        var identity = new ClaimsIdentity(new[]
        {
            accessToken.Role(),
        }, "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
