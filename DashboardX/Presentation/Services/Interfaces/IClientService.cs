using Infrastructure;
using Presentation.Models;
using Shared.Models.Brokers;

namespace Presentation.Services.Interfaces; 

public interface IClientService
{
    Task<Result<List<Client>>> GetClients();
    Task<Result<Client>> GetClient(string brokerId);
    Task<Result> RemoveClient(string brokerId);
    Task<Result<Client>> CreateClient(Broker broker);
    Task<Result<Client>> UpdateClient(Broker broker);
}

