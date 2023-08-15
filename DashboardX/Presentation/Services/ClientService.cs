using Core.Interfaces;
using Infrastructure;
using Presentation.Models;
using Presentation.Services.Interfaces;
using Shared.Models.Brokers;

namespace Presentation.Services;

public class ClientService : IClientService
{
    private readonly IBrokerService _brokerService;
    private readonly ITopicService _topicService;
    private readonly IDeviceService _deviceService;

    private List<BrokerClient> _brokers = new();
    private List<DeviceClient> _devices = new();

    public ClientService(ITopicService topicService, IBrokerService brokerService, IDeviceService deviceService)
    {
        _brokerService = brokerService;
        _topicService = topicService;
        _deviceService = deviceService;
    }

    public async Task<Result<List<BrokerClient>>> GetBrokers()
    {
        var result = await _brokerService.GetBrokers();

        if(!result.Succeeded)
            return Result<List<BrokerClient>>.Fail(result.StatusCode, result.Messages);

        //TODO: UPDATE Brokers list 
        _brokers = result.Data.Select(x => new BrokerClient { Broker = x}).ToList();

        return Result<List<BrokerClient>>.Success(result.StatusCode, _brokers);
    }

    public async Task<Result<BrokerClient>> GetBroker(string brokerId)
    {
        var result = await _brokerService.GetBroker(brokerId);

        if (!result.Succeeded)
            return Result<BrokerClient>.Fail(result.StatusCode, result.Messages);

        //TODO: UPDATE Brokers list 
        var data = new BrokerClient { Broker = result.Data };

        return Result<BrokerClient>.Success(result.StatusCode, data);
    }

    public async Task<Result> RemoveBroker(string brokerId)
    {
        var result = await _brokerService.RemoveBroker(brokerId);

        if(result.Succeeded)
        {
            //TODO: disconnect
            var index = _brokers.FindIndex(x => x.Broker.Id == brokerId);

            if(index != -1)
                _brokers.RemoveAt(index);

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.StatusCode, result.Messages);
    }

    public async Task<Result> CreateBroker(Broker broker)
    {
        var result = await _brokerService.CreateBroker(broker);

        if(result.Succeeded)
        {
            //INITIALIZE ?? 

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.StatusCode, result.Messages);
    }

    public async Task<Result> UpdateBroker(Broker broker)
    {
        var result = await _brokerService.CreateBroker(broker);

        if (result.Succeeded)
        {
            // TODO: Update

            return Result.Success(result.StatusCode);
        }

        return Result.Fail(result.StatusCode, result.Messages);
    }
}
