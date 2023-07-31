using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Constraints.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Infrastructure;

public class ApplicationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    private AuthenticationState currentState;

    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }

    public ApplicationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
        currentState = NoAuthState();
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrWhiteSpace(AccessToken) || string.IsNullOrWhiteSpace(RefreshToken))
            return Task.FromResult(NoAuthState());
        
        return Task.FromResult(currentState);
    }

    public async Task Login(string accessToken, string refreshToken)
    {
        await SetAuthState(accessToken, refreshToken);

        NotifyAuthenticationStateChanged(Task.FromResult(currentState));
    }

    public async Task ExtendSession(string accessToken, string refreshToken) => await SetAuthState(accessToken, refreshToken);

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync(AuthStorageConstraints.AccessToken);
        await _localStorage.RemoveItemAsync(AuthStorageConstraints.RefreshToken);

        currentState = NoAuthState();

        NotifyAuthenticationStateChanged(Task.FromResult(currentState));
    }

    #region State
    private async Task SetAuthState(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        await _localStorage.SetItemAsync(AuthStorageConstraints.AccessToken, accessToken);
        await _localStorage.SetItemAsync(AuthStorageConstraints.RefreshToken, refreshToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        currentState = AuthState(accessToken);
    }
    private static AuthenticationState NoAuthState() => new(new ClaimsPrincipal(new ClaimsIdentity()));
    private static AuthenticationState AuthState(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var claims = jwtToken.Claims;
        var role = UserRole(claims);

        claims = claims.Append(role);

        var identity = new ClaimsIdentity(claims, "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    private static Claim UserRole(IEnumerable<Claim> claims)
    {
        if(IsAdmin(claims))
            return new Claim(ClaimTypes.Role, RolesConstraints.Admin);

        return new Claim(ClaimTypes.Role, RolesConstraints.User);
    }
    private static bool IsAdmin(IEnumerable<Claim> claims) => claims.FirstOrDefault(c => c.Type == "isAdmin")?.Value == "true";

    #endregion
}
