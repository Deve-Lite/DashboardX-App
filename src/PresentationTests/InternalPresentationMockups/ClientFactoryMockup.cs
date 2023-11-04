
using PresentationTests.InternalPresentationMockups;

namespace PresentationTests.InternalServiceMockups;

internal class ClientFactoryMockup : IClientFactory
{
    public IClient GenerateClient(Broker broker)
    {
        return new ClientMockup(broker);
    }
}
