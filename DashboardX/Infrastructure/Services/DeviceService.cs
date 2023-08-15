using Blazored.LocalStorage;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Constraints;
using Shared.Models.Devices;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Infrastructure.Extensions;

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

    public async Task<IResult<Device>> GetDevice(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices/{id}"
        };

        var response = await SendAsync<Device>(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, response.Data);
        }

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var list = await _localStorage.GetItemAsync<List<Device>>(DeviceConstants.DevicesListName);
            response.Data = list.SingleOrDefault(b => b.BrokerId == id)!;
        }

        return response;
    }

    public async Task<IResult<List<Device>>> GetDevices()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "api/v1/devices"
        };

        var response = await SendAsync<List<Device>>(request);

        if (response.StatusCode == HttpStatusCode.OK)
            await _localStorage.SetItemAsync(DeviceConstants.DevicesListName, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<List<Device>>(DeviceConstants.DevicesListName);

        return response;
    }

    public async Task<IResult> CreateDevices(Device device)
    {
        var request = new Request<Device>
        {
            Method = HttpMethod.Post,
            Route = $"api/v1/devices",
            Data = device
        };

        var response = await SendAsync<Device, Device>(request);

        if (response.Succeeded)
        {
            device.BrokerId = response.Data.Id;

            await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, response.Data);

            response.Data = device;
        }

        return response;
    }

    public async Task<IResult> DeleteDevice(string deviceId)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/brokers/{deviceId}"
        };

        var response = await SendAsync(request);

        if (response.Succeeded)
            await _localStorage.RemoveItemFromList<Device>(DeviceConstants.DevicesListName, deviceId);
        
        return response;
    }

    public async Task<IResult> UpdateDevice(Device broker)
    {
        var request = new Request<Device>
        {
            Method = HttpMethod.Put,
            Route = $"api/v1/brokers{broker.BrokerId}",
            Data = broker
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<Device>(request, options);

        if (response.Succeeded)
            await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, broker);

        return response!;
    }
}
