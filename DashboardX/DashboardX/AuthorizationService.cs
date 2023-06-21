using DashboardX.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using System.Net.Http.Headers;

namespace DashboardX.Auth.Services;

public class AuthorizationService : IAuthorizationService
{
    private AccessToken accessToken;
    private RefreshToken refreshToken;
    private readonly HttpClient client;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;
    private readonly TimeSpan maxRequestTime;

    public bool IsAuthenticated => accessToken != null && !string.IsNullOrWhiteSpace(accessToken.Value);

    public AuthorizationService(HttpClient httpClient, 
                                IConfiguration configuration, 
                                NavigationManager  navigationManager, 
                                AuthenticationStateProvider authStateProvider)
    {
        client = httpClient;
        maxRequestTime = TimeSpan.FromSeconds(configuration.GetValue<int>("API:MaxReuestTimeSeconds"));
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;

        //TODO default values
        accessToken = new AccessToken("",maxRequestTime);
        refreshToken = new RefreshToken("", maxRequestTime);
    }

    public void SaveTokens(string accessToken, string refreshToken)
    {
        this.accessToken = new AccessToken(accessToken, maxRequestTime);
        this.refreshToken = new RefreshToken(refreshToken, maxRequestTime);
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

    public  Task Logout()
    {
        // return tokens to api
        //navigate user to login page using _navigationManager
        throw new NotImplementedException();
    }

    public  Task RefreshTokens()
    {
        throw new NotImplementedException();
    }
}
