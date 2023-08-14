using Core.Interfaces;
using Infrastructure;
using Presentation.Models;
using Presentation.Services.Interfaces;

namespace Presentation.Services;

public class ClientService : IClientService
{
    private readonly IBrokerService _brokerService;
    private readonly ITopicService _topicService;
    
    public ClientService(ITopicService topicService, IBrokerService brokerService)
    {
        _brokerService = brokerService;
        _topicService = topicService;
    }


    public Task<Result<List<BrokerClient>>> GetBroker(string brokerId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<BrokerClient>>> GetBrokers()
    {
        throw new NotImplementedException();
    }

    public Task<Result<DeviceClient>> GetDevice(string clientId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<DeviceClient>>> GetDevices()
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<DeviceClient>>> GetDevices(string brokerId)
    {
        throw new NotImplementedException();
    }
}
