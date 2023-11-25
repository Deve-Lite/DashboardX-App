namespace Presentation.Clients;

public interface IClientService
{
    Task Logout();
    Task<IResult<IList<IClient>>> GetClientsWithDevices(bool fetch = true);
    Task<IResult<IList<IClient>>> GetClients(bool fetch = true);
    Task<IResult<IClient>> GetClient(string clientId, bool fetch = true);
}

