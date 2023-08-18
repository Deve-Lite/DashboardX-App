using Infrastructure;
using Presentation.Models;
using Shared.Models.Brokers;

namespace Presentation.Services.Interfaces; 

public interface IClientService
{
    Task<Result<List<BrokerClient>>> GetBrokers();
    Task<Result<BrokerClient>> GetBroker(string brokerId);
    Task<Result> RemoveBroker(string brokerId);
    Task<Result<BrokerClient>> CreateBroker(Broker broker);
    Task<Result<BrokerClient>> UpdateBroker(Broker broker);
}

