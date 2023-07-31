using Core;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models.Auth;

namespace Infrastructe.Services;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly ApplicationStateProvider _applicationStateProvider;

    public AuthenticationService(HttpClient httpClient, 
        AuthenticationStateProvider authenticationStateProvider) : base(httpClient)
    {
        _applicationStateProvider = (ApplicationStateProvider?)authenticationStateProvider!;
    }

    public async Task<Result> Login(LoginRequest data)
    {
        var request = new Request<LoginRequest>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/login",
            Data = data
        };

        var result = await SendAsync<Tokens, LoginRequest>(request);

        if (result.Succeeded)
        {
            var accessToken = result.Data!.AccessToken;
            var refreshToken = result.Data!.RefreshToken;

            await _applicationStateProvider.Login(accessToken, refreshToken);
        }

        return result;
    }

    public async Task<Result> Register(RegisterRequest data)
    {
        var request = new Request<RegisterRequest>
        {
            Method = HttpMethod.Post, 
            Route = "api/v1/users/register", 
            Data = data
        };

        return await SendAsync(request);
    }
}
