using DashboardXModels;
using DashboardXModels.Brokers;

namespace DashboardX.Brokers;

public interface IBrokerService
{
    Task<Response<Broker>> GetBroker(string id);
    Task<Response<List<Broker>>> GetBrokers();
    Task<Response<Broker>> CreateBroker(Broker broker);
    Task<Response<Broker>> UpdateBroker(Broker broker);
    Task<Response> DeleteBroker(string id);
}
