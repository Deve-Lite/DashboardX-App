﻿using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Presentation.Application;

public class ApplicationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ISessionStorageService _sessionStorage;
    private readonly ILogger<ApplicationStateProvider> _logger;

    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }

    public ApplicationStateProvider(HttpClient httpClient, 
        ILocalStorageService localStorage, 
        ISessionStorageService sessionStorage, 
        ILogger<ApplicationStateProvider> logger)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _sessionStorage = sessionStorage;
        _logger = logger;

        AccessToken = string.Empty;
        RefreshToken = string.Empty;
    }


    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrEmpty(AccessToken))
        {
            var rememberUser = await _localStorage.GetItemAsync<bool>(AuthConstraints.RememberMeName);

            AccessToken = await _sessionStorage.GetItemAsync<string>(AuthConstraints.AccessToken);
            RefreshToken = await _sessionStorage.GetItemAsync<string>(AuthConstraints.RefreshToken);

            if (rememberUser)
            {
                AccessToken = await _localStorage.GetItemAsync<string>(AuthConstraints.AccessToken);
                RefreshToken = await _localStorage.GetItemAsync<string>(AuthConstraints.RefreshToken);
            }
        }

        return AuthState(AccessToken);
    }

    public async Task Login(string accessToken, string refreshToken)
    {
        await SaveTokens(accessToken, refreshToken);

        NotifyAuthenticationStateChanged(Task.FromResult(AuthState(AccessToken)));
    }

    public async Task ExtendSession(string accessToken, string refreshToken) => await SaveTokens(accessToken, refreshToken);

    public async Task Logout()
    {
        await _sessionStorage.RemoveItemAsync(AuthConstraints.AccessToken);
        await _sessionStorage.RemoveItemAsync(AuthConstraints.RefreshToken);

        await _localStorage.RemoveItemAsync(AuthConstraints.AccessToken);
        await _localStorage.RemoveItemAsync(AuthConstraints.RefreshToken);

        await _localStorage!.RemoveItemAsync(BrokerConstraints.BrokerListName);
        await _localStorage!.RemoveItemAsync(DeviceConstants.DevicesListName);
        await _localStorage!.RemoveItemAsync(UserConstraints.PreferencesStorage);

        AccessToken = string.Empty;
        RefreshToken = string.Empty;

        NotifyAuthenticationStateChanged(Task.FromResult(NoAuthState()));
    }

    #region State
    private async Task SaveTokens(string accessToken, string refreshToken)
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
    private AuthenticationState AuthState(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return NoAuthState();

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            var role = UserRole(claims);
            claims = claims.Append(role);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }
        catch (Exception ex)
        {
#if DEBUG
            _logger.LogError("Authentication error Message:", ex.Message);
#endif
            _logger.LogError("Couldn't authenticate user.", nameof(ex));
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }

    private static AuthenticationState NoAuthState() => new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static Claim UserRole(IEnumerable<Claim> claims)
    {
        //TODO: skips admin role! 
        if(claims.FirstOrDefault(c => c.Type.ToString() == "is_admin")?.Value == "true")
            return new Claim(ClaimTypes.Role, RolesConstraints.Admin);

        return new Claim(ClaimTypes.Role, RolesConstraints.User);
    }
    #endregion
}
