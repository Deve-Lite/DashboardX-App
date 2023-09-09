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

    public async Task<IResult> DeleteUser()
    {
        //TODO: Add password as a confirmation

        var request = new Request<User>
        {
            Method = HttpMethod.Delete,
            Route = "users/me"
        };

        var response = await SendAsync(request);

        return response;
    }

    public async Task<IResult<User>> GetUser()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "users/me"
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

    public async Task<IResult> UpdateUser(User user)
    {
        var request = new Request<User>
        {
            Method = HttpMethod.Patch,
            Route = "users/me",
            Data = user
        };

        var response = await SendAsync(request);

        if (response.Succeeded)
        {
            var preferences = user.GetPreferences();
            await _prefrenceService.UpdatePreferences(preferences);
        }

        return response;
    }
}
