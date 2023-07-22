using DashboardXModels;

namespace DashboardX.Services.Interfaces;

public interface IClientService
{
    Task<IList<InitializedBroker>> GetBrokers();
    Task<InitializedBroker> GetBroker(string id);
}
