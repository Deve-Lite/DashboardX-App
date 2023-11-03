namespace Presentation.Clients;

public interface IClientFactory
{
    IClient GenerateClient(Broker broker);
}
