﻿using Core.App.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.Users;

public class UserService : AuthorizedService, IUserService
{
    private readonly IPrefrenceService _prefrenceService;

    public UserService(HttpClient httpClient,
                       ILogger<UserService> logger,
                       IPrefrenceService preferenceService,
                       IAuthorizationManager authorizationManager)
        : base(httpClient, logger, authorizationManager)
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

        return await SendAsync(request);
    }

    public async Task<IResult<User>> GetUser()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "api/v1/users/me"
        };

        var response = await SendAsync<User>(request);

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
            Route = "api/v1/users/me/password",
            Data = dto
        };

        return await SendAsync<ChangePasswordModel>(request);
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
            await _prefrenceService.UpdatePreferences(dto);

        return response;
    }
}
