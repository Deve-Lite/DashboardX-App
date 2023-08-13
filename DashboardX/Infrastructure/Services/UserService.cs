using Blazored.LocalStorage;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Constraints.Users;
using Shared.Models.Users;
using System.Net;

namespace Infrastructure.Services;

public class UserService : AuthorizedService, IUserService
{
    public UserService(HttpClient httpClient, 
        ILocalStorageService localStorageService,
        NavigationManager navigationManager, 
        AuthenticationStateProvider authenticationState) 
        : base(httpClient, localStorageService, navigationManager, authenticationState)
    {
    }

    public async Task<IResult> DeleteUser()
    {
        //TODO - Add password as a confirmation? 

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
            await _localStorage.SetItemAsync(UserConstraints.SettingsStorage, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<User>(UserConstraints.UserStorage);

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

        return response;
    }
}
