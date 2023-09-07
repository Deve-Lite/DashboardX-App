using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Constraints;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Security.Claims;

namespace Infrastructure;

public class ApplicationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ISessionStorageService _sessionStorage;

    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }

    public ApplicationStateProvider(HttpClient httpClient, ILocalStorageService localStorage, ISessionStorageService sessionStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _sessionStorage = sessionStorage;
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!string.IsNullOrWhiteSpace(AccessToken))
            return Task.FromResult(AuthState());
        
        return Task.FromResult(NoAuthState());
    }

    public async Task Login(string accessToken, string refreshToken)
    {


        await SetAuthState(accessToken, refreshToken);

        NotifyAuthenticationStateChanged(Task.FromResult(AuthState()));
    }

    public async Task ExtendSession(string accessToken, string refreshToken) => await SetAuthState(accessToken, refreshToken);

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync(AuthConstraints.AccessToken);
        await _localStorage.RemoveItemAsync(AuthConstraints.RefreshToken);

        NotifyAuthenticationStateChanged(Task.FromResult(NoAuthState()));
    }

    public Task RemoveLoginState()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(NoAuthState()));
        return Task.CompletedTask;
    }

    #region State
    private async Task SetAuthState(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;

        var rememberUser = await _localStorage.GetItemAsync<bool>(AuthConstraints.RememberMeName);

        await _sessionStorage.SetItemAsync(AuthConstraints.AccessToken, accessToken);
        await _sessionStorage.SetItemAsync(AuthConstraints.RefreshToken, refreshToken);

        if (rememberUser)
        {
            await _localStorage.SetItemAsync(AuthConstraints.AccessToken, accessToken);
            await _localStorage.SetItemAsync(AuthConstraints.RefreshToken, refreshToken);
        }
        else
        {
            await _localStorage.RemoveItemAsync(AuthConstraints.AccessToken);
            await _localStorage.RemoveItemAsync(AuthConstraints.RefreshToken);
        }
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
    private static AuthenticationState NoAuthState() => new(new ClaimsPrincipal(new ClaimsIdentity()));
    private AuthenticationState AuthState()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(AccessToken);
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
