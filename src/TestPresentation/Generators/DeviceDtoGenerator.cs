
using Bogus;

namespace PresentationTests.Generators;

internal class DeviceDtoGenerator
{
    private static DeviceDTO? firstDevice;
    public static DeviceDTO FirstDevice()
    {
        if (firstDevice == null)
        {
            var broker = BrokerDtoGenerator.FirstBroker();
            firstDevice = GenerateDeviceDto();
            firstDevice.BrokerId = broker.Id;
        }
        return firstDevice;
    }

    private static DeviceDTO? secondDevice;
    public static DeviceDTO SecondDevice()
    {
        if (secondDevice == null)
        {
            var broker = BrokerDtoGenerator.SecondBroker();
            secondDevice = GenerateDeviceDto();
            secondDevice.BrokerId = broker.Id;
        }
        return secondDevice;
    }

    public static DeviceDTO GenerateDeviceDto()
    {
        var iconFaker = new Faker<Icon>()
            .RuleFor(i => i.BackgroundHex, f => f.Internet.Color())
            .RuleFor(i => i.Name, f => f.Random.String(15));

        return new Faker<DeviceDTO>()
            .RuleFor(b => b.Name, f => f.Company.CompanyName())
            .RuleFor(b => b.BaseDevicePath, f => f.Random.String(0, 10))
            .RuleFor(b => b.Placing, f => f.Random.String(15))
            .RuleFor(b => b.Icon, iconFaker.Generate()).RuleFor(b => b.Id, Guid.NewGuid().ToString())
            .Generate();
    }
}
