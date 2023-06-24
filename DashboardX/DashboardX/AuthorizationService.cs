using Blazored.LocalStorage;
using DashboardX.Tokens;
using DashboardXModels.Auth.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace DashboardX;

public class AuthorizationService : BaseService, IAuthorizationService
{
    private readonly HttpClient _client;
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
                                NavigationManager  navigationManager, 
                                AuthenticationStateProvider authStateProvider,
                                ILocalStorageService localStorage) : base(client)
    {
        _client = client;
        maxRequestTime = TimeSpan.FromSeconds(configuration.GetValue<int>("API:MaxReuestTimeSeconds"));
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;


        accessToken = new AccessToken();
        refreshToken = new RefreshToken();
    }

    public void SaveTokens(string accessToken, string refreshToken)
    {
        this.accessToken = new AccessToken(accessToken, maxRequestTime);
        this.refreshToken = new RefreshToken(refreshToken, maxRequestTime);
    }

    public void SaveTokens(AccessToken accessToken, RefreshToken refreshToken)
    {
        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
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

        await _localStorage.RemoveItemAsync(Token.AccessTokenName);
        await _localStorage.RemoveItemAsync(Token.RefreshTokenName);
    }

    public async Task RefreshTokens()
    {

        var tokens = await SendAsync<TokenDTO>(null);

        if (tokens.Success)
        {
            accessToken = new AccessToken(tokens.Data.AccessToken, maxRequestTime);
            refreshToken = new RefreshToken(tokens.Data.RefreshToken, maxRequestTime);

            await _localStorage.SetItemAsync(Token.AccessTokenName, accessToken.Value);
            await _localStorage.SetItemAsync(Token.RefreshTokenName, refreshToken.Value);
        }
        else
        {
            await _localStorage.RemoveItemAsync(Token.AccessTokenName);
            await _localStorage.RemoveItemAsync(Token.RefreshTokenName);

            await _authStateProvider.GetAuthenticationStateAsync();
            _navigationManager.NavigateTo("/auth/login");
        }
    }

}
