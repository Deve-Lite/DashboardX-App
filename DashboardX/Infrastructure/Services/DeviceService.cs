
using Blazored.LocalStorage;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models.Devices;

namespace Infrastructure.Services;

public class DeviceService : AuthorizedService, IDeviceService
{
    public DeviceService(HttpClient httpClient, 
                         ILocalStorageService localStorageService, 
                         NavigationManager navigationManager, 
                         AuthenticationStateProvider authenticationState)
        : base(httpClient, localStorageService, navigationManager, authenticationState)
    {
    }

    public Task<IResult<Device>> CreateDevices(Device broker)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> DeleteDevice(string deviceId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult<Device>> GetDevice(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IResult<List<Device>>> GetDevices(string brokerId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult<List<Device>>> GetDevices()
    {
        throw new NotImplementedException();
    }

    public Task<IResult<Device>> UpdateDevices(Device broker)
    {
        throw new NotImplementedException();
    }
}
