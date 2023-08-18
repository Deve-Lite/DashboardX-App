using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Shared.Models.Brokers;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Core.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Core;
using Shared.Models.Devices;
using Shared.Constraints;
using Infrastructure.Extensions;

namespace Infrastructure.Services;

public class BrokerService : AuthorizedService, IBrokerService
{
    public BrokerService(HttpClient httpClient, 
                         ILocalStorageService localStorageService, 
                         NavigationManager navigationManager, 
                         AuthenticationStateProvider authenticationState) 
        : base(httpClient, localStorageService, navigationManager, authenticationState)
    {
    }

    public async Task<IResult<List<Device>>> GetBrokerDevices(string brokerId)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/brokers/{brokerId}/devices"
        };

        var response = await SendAsync<List<Device>>(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var list = await _localStorage.GetItemAsync<List<Device>>(BrokerConstraints.DevicesListName);

            foreach(var device in response.Data)
            {
                int index = list.FindIndex(x => x.BrokerId == device.BrokerId);

                if (index != -1)
                    list[index] = device;
                else
                    list.Add(device);
            }

            await _localStorage.SetItemAsync(BrokerConstraints.DevicesListName, response.Data);
        }

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var list = await _localStorage.GetItemAsync<List<Device>>(BrokerConstraints.DevicesListName);
            response.Data = list.Where(x => x.BrokerId == brokerId).ToList();
        }

        return response;
    }

    public async Task<IResult<List<Broker>>> GetBrokers()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "api/v1/brokers"
        };

        var response = await SendAsync<List<Broker>>(request);

        if (response.StatusCode == HttpStatusCode.OK)
            await _localStorage.SetItemAsync(BrokerConstraints.BrokerListName, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<List<Broker>>(BrokerConstraints.BrokerListName);

        return response;
    }

    public async Task<IResult<Broker>> GetBroker(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/brokers/{id}"
        };

        var response = await SendAsync<Broker>(request);

        if (response.StatusCode == HttpStatusCode.OK)
            await _localStorage.UpsertItemToList(BrokerConstraints.BrokerListName, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerConstraints.BrokerListName);
            response.Data = list.SingleOrDefault(b => b.Id == id)!;
        }

        return response;
    }

    public async Task<IResult> RemoveBroker(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/brokers/{id}"
        };

        var response = await SendAsync(request);

        if (response.Succeeded)
            await _localStorage.RemoveItemFromList<Broker>(BrokerConstraints.BrokerListName, id);

        return response;
    }

    public async Task<IResult<Broker>> CreateBroker(Broker broker)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/brokers",
            Data = broker
        };

        var response = await SendAsync<Broker, Broker>(request);

        if (response.Succeeded)
        {
            broker.Id = response.Data.Id;
            await _localStorage.UpsertItemToList(BrokerConstraints.BrokerListName, response.Data);
        }

        return response;
    }

    public async Task<IResult<Broker>> UpdateBroker(Broker broker)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Put,
            Route = $"api/v1/brokers{broker.Id}",
            Data = broker
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<Broker, Broker>(request, options);

        if (response.Succeeded)
            await _localStorage.UpsertItemToList(BrokerConstraints.BrokerListName, broker);
        
        return response!;
    }

    //TODO On Update / Add operations requires edited at and id
}
