using Blazored.LocalStorage;
using DashboardX.Services.Interfaces;
using DashboardX.Tokens;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace DashboardX.Services;

public class AuthorizedBaseService : BaseService, IAuthorizedBaseService
{
    protected readonly IAuthorizationService _authorizationService;
    protected readonly NavigationManager _navigationManager;
    protected readonly ILocalStorageService _localStorage;

    public AuthorizedBaseService(HttpClient httpClient,
                                 IAuthorizationService authorizationService,
                                 NavigationManager navigationManager,
                                 ILocalStorageService localStorage) : base(httpClient)
    {
        _authorizationService = authorizationService;
        _navigationManager = navigationManager;
        _localStorage = localStorage;
    }

    public async Task<Response<T>> SendAuthorizedAsync<T>(Request request, JsonSerializerOptions? options = null) where T : class, new()
    {
        HttpRequestMessage message = CreateMessage(request, options);
        await _authorizationService.AuthorizeMessage(message);

        return await Run<T>(message);
    }

    public async Task<Response> SendAuthorizedAsync(Request request, JsonSerializerOptions? options = null)
    {
        HttpRequestMessage message = CreateMessage(request, options);

        await _authorizationService.AuthorizeMessage(message);

        return await Run(message);
    }

    protected override async Task OnUnauthorised(HttpResponseMessage response)
    {
        await _localStorage.RemoveItemAsync(Token.AccessTokenName);
        await _localStorage.RemoveItemAsync(Token.RefreshTokenName);

        _navigationManager.NavigateTo("/unauthorized", true);
    }
}
