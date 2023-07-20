using Blazored.LocalStorage;
using DashboardX.Services;
using DashboardX.Services.Interfaces;
using DashboardXModels.Brokers;
using Microsoft.AspNetCore.Components;

namespace DashboardX.Devices;


public class DeviceService : AuthorizedBaseService, IDeviceService
{
    public DeviceService(HttpClient httpClient, IAuthorizationService authorizationService, NavigationManager navigationManager, ILocalStorageService localStorage) 
        : base(httpClient, authorizationService, navigationManager, localStorage) { }

    public Task<Response<Broker>> CreateDevices(Broker broker)
    {
        throw new NotImplementedException();
    }

    public Task<Response> DeleteDevices(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Broker>> GetDevice(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<Broker>>> GetDevices(string brokerId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<Broker>>> GetDevices()
    {
        throw new NotImplementedException();
    }

    public Task<Response<Broker>> UpdateDevices(Broker broker)
    {
        throw new NotImplementedException();
    }
}
