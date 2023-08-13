
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Shared.Models.Brokers;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Core.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Core;
using Shared.Constraints.Brokers;

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

    public async Task<IResult<List<Broker>>> GetBrokers()
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "brokers"
        };

        var response = await SendAsync<List<Broker>>(request);

        if (response.StatusCode == HttpStatusCode.OK)
            await _localStorage.SetItemAsync(BrokerServiceConstraints.BrokerListName, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<List<Broker>>(BrokerServiceConstraints.BrokerListName);

        return response;
    }

    public async Task<IResult<Broker>> GetBroker(string id)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Get,
            Route = $"brokers/{id}",
            Data = new Broker
            {
                BrokerId = id
            }
        };

        var response = await SendAsync<Broker, Broker>(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerServiceConstraints.BrokerListName);

            int index = list.FindIndex(broker => broker.BrokerId == id);

            if (index != -1)
                list[index] = response.Data;

            await _localStorage.SetItemAsync(BrokerServiceConstraints.BrokerListName, response.Data);
        }

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerServiceConstraints.BrokerListName);
            response.Data = list.SingleOrDefault(b => b.BrokerId == id)!;
        }

        return response;
    }


    public async Task<IResult<Broker>> CreateBroker(Broker broker)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Post,
            Route = "brokers",
            Data = broker
        };

        var response = await SendAsync<Broker, Broker>(request);

        if (response.Succeeded)
        {
            broker.BrokerId = response.Data.BrokerId;

            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerServiceConstraints.BrokerListName);
            list.Add(broker);
            await _localStorage.SetItemAsync(BrokerServiceConstraints.BrokerListName, list);
            response.Data = broker;
        }

        return response;
    }

    public async Task<IResult> DeleteBroker(string id)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Delete,
            Route = $"brokers/{id}",
            Data = new Broker
            {
                BrokerId = id
            }
        };

        var response = await SendAsync(request);

        if (response.Succeeded)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerServiceConstraints.BrokerListName);
            list.RemoveAll(broker => broker.BrokerId == id);
            await _localStorage.SetItemAsync(BrokerServiceConstraints.BrokerListName, list);
        }

        return response;
    }

    public async Task<IResult<Broker>> UpdateBroker(Broker broker)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Put,
            Route = $"brokers{broker.BrokerId}",
            Data = broker
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<Broker, Broker>(request, options);

        if (response.Succeeded)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerServiceConstraints.BrokerListName);
            int index = list.FindIndex(b => b.BrokerId == broker.BrokerId);
            list[index] = broker;
            await _localStorage.SetItemAsync(BrokerServiceConstraints.BrokerListName, list);
        }

        return response!;
    }

}
