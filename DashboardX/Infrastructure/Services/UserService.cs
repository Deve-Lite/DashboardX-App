using Blazored.LocalStorage;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Shared.Constraints;
using Shared.Models.Users;
using System.Net;

namespace Infrastructure.Services;

public class UserService : AuthorizedService, IUserService
{
    private readonly IPrefrenceService _prefrenceService;

    public UserService(HttpClient httpClient,
        ILocalStorageService localStorageService,
        ILogger<UserService> logger,
        IPrefrenceService preferenceService,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationState)
        : base(httpClient, localStorageService, logger, navigationManager, authenticationState)
    {
        _prefrenceService = preferenceService;
    }

    public async Task<IResult> RemoveAccount(PasswordConfirm dto)
    {
        var request = new Request<PasswordConfirm>
        {
            Method = HttpMethod.Delete,
            Route = "api/v1/users/me",
            Data = dto
        };

        var response = await SendAsync(request);

        return response;
    }

    public async Task<IResult<User>> GetUser()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "api/v1/users/me"
        };

        var response = await SendAsync<User>(request);

        if (response.StatusCode == HttpStatusCode.OK)
            await _localStorage.SetItemAsync(UserConstraints.PreferencesStorage, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<User>(UserConstraints.PreferencesStorage);

        if (response.Succeeded)
        {
            var preferences = response.Data.GetPreferences();
            await _prefrenceService.UpdatePreferences(preferences);
        }

        return response;
    }

    public async Task<IResult> ChangePassword(ChangePasswordModel dto)
    {
        var request = new Request<ChangePasswordModel>
        {
            Method = HttpMethod.Patch,
            Route = "api/v1/users/me",
            Data = dto
        };

        var response = await SendAsync<ChangePasswordModel>(request);

        return response;
    }

    public async Task<IResult> UpdatePreferences(Preferences dto)
    {
        var request = new Request<Preferences>
        {
            Method = HttpMethod.Patch,
            Route = "api/v1/users/me",
            Data = dto
        };

        var response = await SendAsync<Preferences>(request);

        if (response.Succeeded)
            await _prefrenceService.UpdatePreferences(dto);

        if (response.Succeeded)
        {
            var preferences = user.GetPreferences();
            await _prefrenceService.UpdatePreferences(preferences);
        }

        return response;
    }
}
