﻿using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Presentation.Brokers;

public class BrokerService : AuthorizedService, IBrokerService
{
    public BrokerService(HttpClient httpClient,
                         ILocalStorageService localStorageService,
                         ILogger<BrokerService> logger,
                         NavigationManager navigationManager,
                         AuthenticationStateProvider authenticationState)
        : base(httpClient, localStorageService, logger, navigationManager, authenticationState)
    {
    }

    //TODO: Function retriving password and username for broker
    //TODO: Replace function to brokers
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

            foreach (var device in response.Data)
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

    public async Task<IResult<Broker>> CreateBroker(Broker broker)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Post,
            Route = "api/v1/brokers",
            Data = broker
        };

        var response = await SendAsync<BaseModel, Broker>(request);

        if (!response.Succeeded)
            return Result<Broker>.Fail(response.Messages, response.StatusCode);

        var itemResponse = await GetBroker(response.Data.Id);

        if (!itemResponse.Succeeded)
            return Result<Broker>.Fail(itemResponse.StatusCode, itemResponse.Messages + " Pleace refresh page.");

        broker = itemResponse.Data;

        await _localStorage.UpsertItemToList(BrokerConstraints.BrokerListName, broker);

        return Result<Broker>.Success(broker, response.StatusCode);
    }

    public async Task<IResult<Broker>> UpdateBroker(Broker broker)
    {
        var request = new Request<Broker>
        {
            Method = HttpMethod.Patch,
            Route = $"api/v1/brokers/{broker.Id}",
            Data = broker
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<Broker>(request, options);

        if (!response.Succeeded)
            return Result<Broker>.Fail(response.Messages, response.StatusCode);

        var itemResponse = await GetBroker(broker.Id);

        if (!itemResponse.Succeeded)
            return Result<Broker>.Fail(itemResponse.Messages, itemResponse.StatusCode);

        broker = itemResponse.Data;

        await _localStorage.UpsertItemToList(BrokerConstraints.BrokerListName, broker);

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

        if (response.Succeeded)
            await _localStorage.RemoveItemFromList<Broker>(BrokerConstraints.BrokerListName, id);

        return response;
    }
}