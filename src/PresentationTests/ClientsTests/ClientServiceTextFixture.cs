using Microsoft.Extensions.Logging;
using PresentationTests.Generators;
using PresentationTests.InternalServiceMockups;
using PresentationTests.ServiceMockups;

namespace PresentationTests.ClientsTests;

public class ClientServiceTextFixture : IDisposable
{
    public IBrokerService BrokerService { get; private set; }
    public IDeviceService DeviceService { get; private set; }
    public IClientService ClientService { get; private set; }
    public IClientFactory ClientFactory { get; private set; }
    public ILogger<ClientService> Logger { get; private set; }

    public ClientServiceTextFixture()
    {
        BrokerService = new BrokerServiceMockup();
        DeviceService = new DeviceServiceMockup();
        ClientFactory = new ClientFactoryMockup();
        Logger = new Logger<ClientService>(new LoggerFactory());

        SetupServices();

        ClientService = new ClientService(BrokerService, DeviceService, Logger, ClientFactory);
    }

    public void SetupServices() 
    {
        BrokerService.CreateBroker(BrokerDtoGenerator.FirstBroker());
        BrokerService.CreateBroker(BrokerDtoGenerator.SecondBroker());

        DeviceService.CreateDevice(DeviceDtoGenerator.FirstDevice());
        DeviceService.CreateDevice(DeviceDtoGenerator.SecondDevice());

        var listOfControls = new List<List<Control>>()
        {
            new List<Control>()
            {
                ControlGenerator.FirstDeviceControl1(),
                ControlGenerator.FirstDeviceControl2(),
                ControlGenerator.FirstDeviceControl3(),
            },
            new List<Control>()
            {
                ControlGenerator.SecondDeviceControl1(),
                ControlGenerator.SecondDeviceControl2(),
                ControlGenerator.SecondDeviceControl3(),
            }
        };

        foreach(var sublist in listOfControls)
            foreach(var control in sublist)
                DeviceService.CreateDeviceControl(control);
    }


    public void Dispose()
    {
        
    }
}
