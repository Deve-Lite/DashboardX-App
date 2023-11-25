using Microsoft.AspNetCore.Components;
using Presentation.Application.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Presentation;

public abstract class AuthorizedService : BaseService
{
    protected readonly ApplicationStateProvider _applicationStateProvider;
    protected readonly NavigationManager _navigationManager;
    protected readonly IAuthenticationManager _authenticationManager;
    
    protected AuthorizedService(HttpClient httpClient,
        ILogger<AuthorizedService> logger,
        IAuthenticationManager authenticationManager) : base(httpClient, logger)
    {
        _authenticationManager = authenticationManager;
    }

    protected virtual async Task<Result> SendAsync(Request request)
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
        var message = CreateMessage(request, options);
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
        var message = CreateMessage(request, options);
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
            Method = HttpMethod.Post,
            Route = "api/v1/users/me/tokens"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationManager.GetRefreshToken());

        var response = await SendAsync<Tokens>(request);

        if(response.Succeeded)
        {
            var accessToken = response.Data!.AccessToken;
            var refreshToken = response.Data!.RefreshToken;
            await _authenticationManager.ExtendSession(accessToken, refreshToken);
            return true;
        }

        _authenticationManager?.Logout();

        return false;
    }
}