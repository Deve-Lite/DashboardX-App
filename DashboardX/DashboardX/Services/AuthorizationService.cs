using Blazored.LocalStorage;
using DashboardX.Services.Interfaces;
using DashboardX.Tokens;
using DashboardXModels.Auth.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace DashboardX.Services;

public class AuthorizationService : BaseService, IAuthorizationService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;
    private readonly ILocalStorageService _localStorage;

    private AccessToken accessToken;
    public AccessToken AccessToken => accessToken;

    private RefreshToken refreshToken;
    public RefreshToken RefreshToken => refreshToken;

    private readonly TimeSpan maxRequestTime;
    public TimeSpan MaxRequestTime => maxRequestTime;

    public AuthorizationService(HttpClient client,
                                IConfiguration configuration,
                                NavigationManager navigationManager,
                                AuthenticationStateProvider authStateProvider,
                                ILocalStorageService localStorage) : base(client, configuration)
    {
        maxRequestTime = TimeSpan.FromSeconds(configuration.GetValue<int>("Api:MaxRequestTimeSeconds"));
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;


        accessToken = new AccessToken();
        refreshToken = new RefreshToken();
    }

    public void AuthenticateSession(string accessToken, string refreshToken)
    {
        this.accessToken = new AccessToken(accessToken, maxRequestTime);
        this.refreshToken = new RefreshToken(refreshToken, maxRequestTime);

        (_authStateProvider as AuthStateProvider)!.NotifyUserLoggedIn(this.accessToken);
    }

    public void AuthenticateSession(AccessToken accessToken, RefreshToken refreshToken)
    {
        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
    }

    public async Task SaveTokensInStorage()
    {
        await _localStorage.SetItemAsync(Token.AccessTokenName, accessToken.Value);
        await _localStorage.SetItemAsync(Token.RefreshTokenName, refreshToken);
    }

    public async Task RemoveTokensFromStorage()
    {
        await _localStorage.RemoveItemAsync(Token.AccessTokenName);
        await _localStorage.RemoveItemAsync(Token.RefreshTokenName);

        (_authStateProvider as AuthStateProvider)!.NotifyUserLogout();
    }

    public async Task AuthorizeClient(HttpClient httpClient)
    {
        if (accessToken.RequiresRefresh)
            await RefreshTokens();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken.Value);
    }

    public async Task AuthorizeMessage(HttpRequestMessage message)
    {
        if (accessToken.RequiresRefresh)
            await RefreshTokens();

        message.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken.Value);
    }

    public async Task Logout()
    {
        var data = new TokenDTO
        {
            RefreshToken = refreshToken.Value,
            AccessToken = accessToken.Value
        };

        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "auth/logout",
            Data = data
        };

        await SendAsync(request);

        await RemoveTokensFromStorage();
    }

    public async Task RefreshTokens()
    {
        var oldTokens = new TokenDTO
        {
            RefreshToken = refreshToken.Value,
            AccessToken = accessToken.Value
        };

        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "auth/refresh",
            Data = oldTokens
        };

        var tokens = await SendAsync<TokenDTO>(request);

        if (tokens.Success)
        {
            AuthenticateSession(tokens.Data.AccessToken, tokens.Data.RefreshToken);
            await SaveTokensInStorage();
        }
        else
        {
            await RemoveTokensFromStorage();
            _navigationManager.NavigateTo("/auth/login");
        }
    }

}
