
using Shared.Models.Brokers;
using Shared.Models.Devices;

namespace Core.Interfaces;

public interface IBrokerService
{
    Task<IResult<Broker>> GetBroker(string id);
    Task<IResult<List<Broker>>> GetBrokers();
    Task<IResult<List<Device>>> GetBrokerDevices(string brokerId);
    Task<IResult> CreateBroker(Broker broker);
    Task<IResult> UpdateBroker(Broker broker);
    Task<IResult> RemoveBroker(string id);
}
