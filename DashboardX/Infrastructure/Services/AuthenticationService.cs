using Core;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models.Auth;
using System.Net.Http.Headers;

namespace Infrastructure.Services;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly ApplicationStateProvider _applicationStateProvider;

    public AuthenticationService(HttpClient httpClient, 
        AuthenticationStateProvider authenticationStateProvider) : base(httpClient)
    {
        _applicationStateProvider = (ApplicationStateProvider?)authenticationStateProvider!;
    }

    public async Task<IResult> Login(LoginModel data)
    {
        var request = new Request<LoginModel>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/login",
            Data = data
        };

        var result = await SendAsync<Tokens, LoginModel>(request);

        if (result.Succeeded)
        {
            var accessToken = result.Data!.AccessToken;
            var refreshToken = result.Data!.RefreshToken;

            await _applicationStateProvider.Login(accessToken, refreshToken);
        }

        return result;
    }

    public async Task<IResult> Register(RegisterModel data)
    {
        var request = new Request<RegisterModel>
        {
            Method = HttpMethod.Post, 
            Route = "api/v1/users/register", 
            Data = data
        };

        return await SendAsync(request);
    }

    public async Task<bool> AuthenticateOnRememberMe(string currentRefreshToken)
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "api/v1/users/refresh"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentRefreshToken);

        var response = await SendAsync<Tokens>(request);

        if (response.Succeeded)
        {
            var accessToken = response.Data!.AccessToken;
            var refreshToken = response.Data!.RefreshToken;
            await _applicationStateProvider.Login(accessToken, refreshToken);

            return true;
        }

        return false;
    }
}
