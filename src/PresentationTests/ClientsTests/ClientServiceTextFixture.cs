using PresentationTests.ServiceMockups;

namespace PresentationTests.ClientsTests;

public class ClientServiceTextFixture : IDisposable
{
    private BrokerServiceMockup _brokerServiceMockup;
    public BrokerServiceMockup BrokerServiceMockup => _brokerServiceMockup;

    public ClientServiceTextFixture()
    {
        _brokerServiceMockup = new BrokerServiceMockup();
        //TODO: Broker Generator 

    }

    public void Dispose()
    {
        
    }
}
