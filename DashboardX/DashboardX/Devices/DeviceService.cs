using Blazored.LocalStorage;
using DashboardX.Services;
using DashboardX.Services.Interfaces;
using DashboardXModels.Devices;
using Microsoft.AspNetCore.Components;

namespace DashboardX.Devices;


public class DeviceService : AuthorizedBaseService, IDeviceService
{
    public DeviceService(HttpClient httpClient, 
                         IAuthorizationService authorizationService,
                         IConfiguration configuration,   
                         NavigationManager navigationManager, 
                         ILocalStorageService localStorage) : base(httpClient, authorizationService, configuration, navigationManager, localStorage) { }

    public Task<Response<Device>> CreateDevices(Device broker)
    {
        throw new NotImplementedException();
    }

    public Task<Response> DeleteDevices(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Device>> GetDevice(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<Device>>> GetDevices(string brokerId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<Device>>> GetDevices()
    {
        throw new NotImplementedException();
    }

    public Task<Response<Device>> UpdateDevices(Device broker)
    {
        throw new NotImplementedException();
    }
}
