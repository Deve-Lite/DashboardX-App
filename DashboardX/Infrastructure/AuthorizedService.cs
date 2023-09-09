using Blazored.LocalStorage;
using Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Shared.Models.Auth;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Infrastructure;

public abstract class AuthorizedService : BaseService
{
    protected readonly ILocalStorageService _localStorage;
    protected readonly ApplicationStateProvider _applicationStateProvider;
    protected readonly NavigationManager _navigationManager;
    
    protected AuthorizedService(HttpClient httpClient, 
        ILocalStorageService localStorageService, 
        ILogger<AuthorizedService> logger,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationState) : base(httpClient, logger)
    {
        _navigationManager = navigationManager;
        _localStorage = localStorageService;
        _applicationStateProvider = (ApplicationStateProvider)authenticationState;
    }

    protected virtual async Task<Result> SendAsync(Request request, JsonSerializerOptions? options = null)
    {
        var message = CreateMessage(request);
        var results = await Run(message);

        if (results.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (!await RefreshTokens())
                return Result.Fail(results.StatusCode);

            results = await Run(message);
        }

        return results;
    }

    protected override async Task<Result<T>> SendAsync<T>(Request request, JsonSerializerOptions? options = null) where T : class
    {
        var message = CreateMessage(request);
        var results = await Run<T>(message);

        if(results.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (!await RefreshTokens())
                return Result<T>.Fail(results.StatusCode);

            results = await Run<T>(message);
        }

        return results;
    }

    protected override async Task<Result> SendAsync<T>(Request<T> request, JsonSerializerOptions? options = null) where T : class
    {
        var message = CreateMessage(request);
        var results = await Run(message);

        if (results.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (!await RefreshTokens())
                return Result.Fail(results.StatusCode);

            results = await Run(message);
        }

        return results;
    }

    protected override async Task<Result<T>> SendAsync<T, T1>(Request<T1> request, JsonSerializerOptions? options = null) where T1 : class where T : class
    {
        var message = CreateMessage(request);
        var results = await Run<T>(message);

        if (results.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (!await RefreshTokens())
                return Result<T>.Fail(results.StatusCode);

            results = await Run<T>(message);
        }

        return results;
    }

    private async Task<bool> RefreshTokens()
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = "users/me"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _applicationStateProvider.RefreshToken);

        var response = await SendAsync<Tokens>(request);

        if(response.Succeeded)
        {
            var accessToken = response.Data!.AccessToken;
            var refreshToken = response.Data!.RefreshToken;
            await _applicationStateProvider.ExtendSession(accessToken, refreshToken);

            return true;
        }

        //TODO: Handle this better, mayby with special page describing error.
        _navigationManager.NavigateTo("/");

        return false;
    }
}