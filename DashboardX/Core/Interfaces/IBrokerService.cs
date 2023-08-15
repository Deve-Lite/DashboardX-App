
using Shared.Models.Brokers;
using Shared.Models.Devices;

namespace Core.Interfaces;

public interface IBrokerService
{
    Task<IResult<Broker>> GetBroker(string id);
    Task<IResult<List<Broker>>> GetBrokers();
    Task<IResult<List<Device>>> GetBrokerDevices(string brokerId);
    Task<IResult<Broker>> CreateBroker(Broker broker);
    Task<IResult<Broker>> UpdateBroker(Broker broker);
    Task<IResult> DeleteBroker(string id);
}
