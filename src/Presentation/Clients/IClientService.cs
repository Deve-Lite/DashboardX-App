namespace Presentation.Clients;

public interface IClientService
{
    Task Logout();

    Task<IResult<IList<IClient>>> GetClientsWithDevices();
    Task<IResult<IList<IClient>>> GetClients();
    Task<IResult<IClient>> GetClient(string clientId);
    Task<IResult> RemoveClient(string clientId);
    Task<IResult> CreateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);
    Task<IResult> UpdateClient(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);
}

