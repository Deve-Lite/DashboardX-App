using Core.App.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Core;

public abstract class AuthorizedService : BaseService
{
    protected readonly IAuthorizationManager _authorizationManager;
    
    protected AuthorizedService(HttpClient httpClient,
        ILogger<AuthorizedService> logger,
        IAuthorizationManager authorizationManager) : base(httpClient, logger)
    {
        _authorizationManager = authorizationManager;
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

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationManager.GetRefreshToken());

        var response = await SendAsync<Tokens>(request);

        if(response.Succeeded)
        {
            var accessToken = response.Data!.AccessToken;
            var refreshToken = response.Data!.RefreshToken;
            await _authorizationManager.ExtendSession(accessToken, refreshToken);
            return true;
        }

        _authorizationManager?.Logout();

        return false;
    }
}