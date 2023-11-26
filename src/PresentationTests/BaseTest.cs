namespace PresentationTests;

using Microsoft.Extensions.Logging;

public class BaseTest
{
    public IFetchBrokerService FetchBrokerService { get; private set; }
    public IFetchDeviceService FetchDeviceService { get; private set; }
    public IFetchControlService FetchControlService { get; private set; }
    public IClientService ClientService { get; private set; }
    public IClientManager ClientManager { get; private set; }
    public ILogger<ClientService> Logger { get; private set; }

    public BaseTest()
    {
        FetchBrokerService = new FetchBrokerServiceMockup();
        FetchDeviceService = new FetchDeviceServiceMockup();
        FetchControlService = new FetchControlServiceMockup();
        ClientManager = new ClientManagerMockup();
        Logger = new Logger<ClientService>(new LoggerFactory());

        ClientService = new ClientService(FetchBrokerService, FetchDeviceService, FetchControlService, ClientManager, Logger);
    }

    public async Task Setup()
    {
        await FetchBrokerService.CreateBroker(BrokerDtoGenerator.FirstBroker());
        await FetchBrokerService.CreateBroker(BrokerDtoGenerator.SecondBroker());

        await FetchDeviceService.CreateDevice(DeviceDtoGenerator.FirstDevice());
        await FetchDeviceService.CreateDevice(DeviceDtoGenerator.SecondDevice());

        var listOfControls = new List<List<ControlDTO>>()
        {
            new List<ControlDTO>()
            {
                ControlGenerator.FirstDeviceControl1(),
                ControlGenerator.FirstDeviceControl2(),
                ControlGenerator.FirstDeviceControl3(),
            },
            new List<ControlDTO>()
            {
                ControlGenerator.SecondDeviceControl1(),
                ControlGenerator.SecondDeviceControl2(),
                ControlGenerator.SecondDeviceControl3(),
            }
        };

        foreach (var sublist in listOfControls)
            foreach (var control in sublist)
                await FetchControlService.CreateControl(control);
    }

    public void TearDown()
    {
        FetchBrokerService = new FetchBrokerServiceMockup();
        FetchDeviceService = new FetchDeviceServiceMockup();
        FetchControlService = new FetchControlServiceMockup();
        ClientManager = new ClientManagerMockup();
        Logger = new Logger<ClientService>(new LoggerFactory());
        ClientService = new ClientService(FetchBrokerService, FetchDeviceService, FetchControlService, ClientManager, Logger);

    }
}
