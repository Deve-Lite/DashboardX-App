using Infrastructure;
using Presentation.Models;
using Shared.Models.Brokers;

namespace Presentation.Services.Interfaces; 

public interface IClientService
{
    Task<Result<List<BrokerClient>>> GetBrokers();
    Task<Result<BrokerClient>> GetBroker(string brokerId);
    Task<Result> RemoveBroker(string brokerId);
    Task<Result> CreateBroker(Broker broker);
    Task<Result> UpdateBroker(Broker broker);
}

