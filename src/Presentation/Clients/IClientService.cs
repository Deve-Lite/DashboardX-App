namespace Presentation.Clients;

public interface IClientService
{
    Task Logout();
    Task<IResult<IList<IClient>>> GetClientsWithDevices();
    Task<IResult<IList<IClient>>> GetClients();
    Task<IResult<IClient>> GetClient(string clientId);
}

