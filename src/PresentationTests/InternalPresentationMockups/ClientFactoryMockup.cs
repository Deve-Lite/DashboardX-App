namespace PresentationTests.InternalServiceMockups;

internal class ClientFactoryMockup : IClientManager
{
    public IResult<IClient> AddClient(Broker broker)
    {
        throw new NotImplementedException();
    }

    public IResult<IClient> GetClient(string clientId)
    {
        throw new NotImplementedException();
    }

    public IResult<IList<IClient>> GetClients()
    {
        throw new NotImplementedException();
    }

    public Task<IResult> RemoveClient(string clientId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult<IClient>> UpdateClient(Broker broker)
    {
        throw new NotImplementedException();
    }
}
