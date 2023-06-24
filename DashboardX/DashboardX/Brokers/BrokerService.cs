using Blazored.LocalStorage;
using DashboardXModels.Brokers;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DashboardX.Brokers;

public class BrokerService : AuthorizedBaseService, IBrokerService
{
    public BrokerService(HttpClient httpClient, 
                         IAuthorizationService authorizationService, 
                         NavigationManager navigationManager, 
                         ILocalStorageService localStorage) : base(httpClient, authorizationService, navigationManager, localStorage)
    {
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

        broker.BrokerId = response.Data.BrokerId;

        response.Data = broker;

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

        return await SendAuthorizedAsync(request);
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

        return await SendAuthorizedAsync<Broker>(request);
    }

    public async Task<Response<List<Broker>>> GetBrokers()
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "brokers"
        };

        return await SendAuthorizedAsync<List<Broker>>(request);
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

        return response!;
    }
}
