using Core.App.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Core.Brokers;

public class FetchBrokerService : AuthorizedService, IFetchBrokerService
{
    public FetchBrokerService(HttpClient httpClient,
                              ILogger<FetchBrokerService> logger,
                              IAuthorizationManager authorizationManager)
        : base(httpClient, logger, authorizationManager) { }

    public async Task<IResult<List<Broker>>> GetBrokers()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "api/v1/brokers"
        };

        var response = await SendAsync<List<BrokerDTO>>(request);

        if(response.Succeeded)
        {
            var brokers = response.Data.Select(x => Broker.FromDto(x)).ToList();
            return Result<List<Broker>>.Success(brokers, response.StatusCode);
        }

        return Result<List<Broker>>.Fail(response.StatusCode, response.Messages[0]);
    }

    public async Task<IResult<Broker>> GetBroker(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/brokers/{id}"
        };

        var response = await SendAsync<BrokerDTO>(request);

        if (response.Succeeded)
        {
            var broker = Broker.FromDto(response.Data);
            return Result<Broker>.Success(broker, response.StatusCode);
        }

        return Result<Broker>.Fail(response.StatusCode, response.Messages[0]);
    }

    public async Task<IResult<Broker>> CreateBroker(BrokerDTO dto)
    {
        var request = new Request<BrokerDTO>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/brokers",
            Data = dto
        };

        var response = await SendAsync<BaseModel, BrokerDTO>(request);

        if (!response.Succeeded)
            return Result<Broker>.Fail(response.Messages, response.StatusCode);

        var itemResponse = await GetBroker(response.Data.Id);

        if (!itemResponse.Succeeded)
            return Result<Broker>.Fail(itemResponse.StatusCode, itemResponse.Messages + " Pleace refresh page.");

        var broker = itemResponse.Data;

        return Result<Broker>.Success(broker, response.StatusCode);
    }

    public async Task<IResult<Broker>> UpdateBroker(BrokerDTO dto)
    {
        var request = new Request<BrokerDTO>
        {
            Method = HttpMethod.Patch,
            Route = $"api/v1/brokers/{dto.Id}",
            Data = dto
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<Broker,BrokerDTO>(request, options);

        if (!response.Succeeded)
            return Result<Broker>.Fail(response.Messages, response.StatusCode);

        var itemResponse = await GetBroker(dto.Id);

        if (!itemResponse.Succeeded)
            return Result<Broker>.Fail(itemResponse.Messages, itemResponse.StatusCode);

        var broker = itemResponse.Data;

        return Result<Broker>.Success(broker, response.StatusCode);
    }

    public async Task<IResult> RemoveBroker(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/brokers/{id}"
        };

        var response = await SendAsync(request);

        return response;
    }

    public async Task<IResult> UpdateBrokerCredentials(string brokerId, BrokerCredentialsDTO dto)
    {
        var request = new Request<BrokerCredentialsDTO>
        {
            Method = HttpMethod.Put,
            Route = $"api/v1/brokers/{brokerId}/credentials",
            Data = dto
        };

        return await SendAsync<BrokerCredentialsDTO>(request);
    }

    public async Task<IResult<BrokerCredentialsDTO>> GetBrokerCredentials(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/brokers/{id}/credentials"
        };

        return await SendAsync<BrokerCredentialsDTO>(request);
    }
}
