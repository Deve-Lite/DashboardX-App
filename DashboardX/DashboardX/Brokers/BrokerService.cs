using DashboardX.Auth.Services;
using DashboardXModels.Brokers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DashboardX.Brokers;

public class BrokerService : BaseService, IBrokerService
{
    public BrokerService(HttpClient httpClient, IAuthorizationService authorizationService) : base(httpClient, authorizationService)
    {
    }

    public async Task<Response<Broker>> CreateBroker(Broker broker)
    {
        var request = new Request
        {
            Method = HttpMethod.Post,
            Route = "api/brokers",
            Data = broker
        };

        var response = await SendAuthorizedAsync<Broker>(request);

        broker.BrokerId = response.Data.BrokerId;
        broker.CreatedAtTicks = response.Data.CreatedAtTicks;
        broker.EditedAtTicks = response.Data.CreatedAtTicks;

        response.Data = broker;

        return response;
    }

    public async Task<Response> DeleteBroker(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/brokers/{id}",
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
            Route = $"api/brokers/{id}",
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
            Route = "api/brokers"
        };

        return await SendAuthorizedAsync<List<Broker>>(request);
    }

    public async Task<Response<Broker>> UpdateBroker(Broker broker)
    {
        var request = new Request
        {
            Method = HttpMethod.Put,
            Route = $"api/brokers{broker.BrokerId}",
            Data = broker
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAuthorizedAsync<Broker>(request);

        broker.EditedAtTicks = response.Data.EditedAtTicks;

        return response!;
    }
}
