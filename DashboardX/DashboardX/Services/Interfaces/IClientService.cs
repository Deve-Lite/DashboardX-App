using DashboardXModels;

namespace DashboardX.Services.Interfaces;

public interface IClientService
{
    Task<IList<InitializedBroker>> GetInitializedBrokers();
    Task<InitializedBroker> GetInitializedBroker(string id);
}
