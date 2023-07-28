using Blazored.LocalStorage;
using DashboardX.Services;
using DashboardX.Services.Interfaces;
using DashboardXModels.Brokers;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DashboardX.Brokers;

public class BrokerService : AuthorizedBaseService, IBrokerService
{
    public static string BrokerListName = "BrokerList";

    public BrokerService(HttpClient httpClient, 
                         IAuthorizationService authorizationService, 
                         IConfiguration configuration,
                         NavigationManager navigationManager, 
                         ILocalStorageService localStorage) : base(httpClient, authorizationService, configuration, navigationManager, localStorage)
    {
        
    }

    public async Task<Response<List<Broker>>> GetBrokers()
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "brokers"
        };

        var response = await SendAuthorizedAsync<List<Broker>>(request);

        if(response.StatusCode == HttpStatusCode.OK)
            await _localStorage.SetItemAsync(BrokerListName, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<List<Broker>>(BrokerListName);

        return response;
    }

    public async Task<Response<Broker>> GetBroker(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"brokers/{id}",
            Data = new Broker
            {
                BrokerId = id
            }
        };

        var response = await SendAuthorizedAsync<Broker>(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerListName);

            int index = list.FindIndex(broker => broker.BrokerId == id);

            if (index != -1)
                list[index] = response.Data;
               
            await _localStorage.SetItemAsync(BrokerListName, response.Data);
        }

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerListName);
            response.Data = list.SingleOrDefault(b => b.BrokerId == id)!;
        }

        return response;
    }


    public async Task<Response<Broker>> CreateBroker(Broker broker)
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "brokers",
            Data = broker
        };

        var response = await SendAuthorizedAsync<Broker>(request);

        if(response.Success)
        {
            broker.BrokerId = response.Data.BrokerId;

            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerListName);
            list.Add(broker);
            await _localStorage.SetItemAsync(BrokerListName, list);
            response.Data = broker;
        }

        return response;
    }

    public async Task<Response> DeleteBroker(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"brokers/{id}",
            Data = new Broker 
            { 
                BrokerId = id
            }
        };

        var response = await SendAuthorizedAsync(request);

        if (response.Success)
        {
             var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerListName);
            list.RemoveAll(broker => broker.BrokerId == id);
            await _localStorage.SetItemAsync(BrokerListName, list);
        }

        return response;
    }

    public async Task<Response<Broker>> UpdateBroker(Broker broker)
    {
        var request = new Request
        {
            Method = HttpMethod.Put,
            Route = $"brokers{broker.BrokerId}",
            Data = broker
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAuthorizedAsync<Broker>(request, options);

        if (response.Success)
        {
            var list = await _localStorage.GetItemAsync<List<Broker>>(BrokerListName);
            int index = list.FindIndex(b => b.BrokerId == broker.BrokerId);
            list[index] = broker;
            await _localStorage.SetItemAsync(BrokerListName, list);
        }

        return response!;
    }
}
