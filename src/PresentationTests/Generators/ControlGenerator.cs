using Bogus;

namespace PresentationTests.Generators;

internal class ControlGenerator
{
    private static Control? firstDeviceControl1;
    public static Control FirstDeviceControl1()
    {
        if (firstDeviceControl1 == null)
        {
            var device = DeviceDtoGenerator.FirstDevice();
            firstDeviceControl1 = GenerateControl();
            firstDeviceControl1.DeviceId = device.Id;
        }
        return firstDeviceControl1;
    }

    private static Control? firstDeviceControl2;
    public static Control FirstDeviceControl2()
    {
        if (firstDeviceControl2 == null)
        {
            var device = DeviceDtoGenerator.FirstDevice();
            firstDeviceControl2 = GenerateControl();
            firstDeviceControl2.DeviceId = device.Id;
        }
        return firstDeviceControl2;
    }

    private static Control? firstDeviceControl3;
    public static Control FirstDeviceControl3()
    {
        if (firstDeviceControl3 == null)
        {
            var device = DeviceDtoGenerator.FirstDevice();
            firstDeviceControl3 = GenerateControl();
            firstDeviceControl3.DeviceId = device.Id;
        }
        return firstDeviceControl3;
    }


    private static Control? secondDeviceControl1;
    public static Control SecondDeviceControl1()
    {
        if (secondDeviceControl1 == null)
        {
            var device = DeviceDtoGenerator.SecondDevice();
            secondDeviceControl1 = GenerateControl();
            secondDeviceControl1.DeviceId = device.Id;
        }
        return secondDeviceControl1;
    }

    private static Control? secondDeviceControl2;
    public static Control SecondDeviceControl2()
    {
        if (secondDeviceControl2 == null)
        {
            var device = DeviceDtoGenerator.SecondDevice();
            secondDeviceControl2 = GenerateControl();
            secondDeviceControl2.DeviceId = device.Id;
        }
        return secondDeviceControl2;
    }

    private static Control? secondDeviceControl3;
    public static Control SecondDeviceControl3()
    {
        if (secondDeviceControl3 == null)
        {
            var device = DeviceDtoGenerator.SecondDevice();
            secondDeviceControl3 = GenerateControl();
            secondDeviceControl3.DeviceId = device.Id;
        }
        return secondDeviceControl3;
    }

    public static Control GenerateControl()
    {
        var iconFaker = new Faker<Icon>()
            .RuleFor(i => i.BackgroundHex, f => f.Internet.Color())
            .RuleFor(i => i.Name, f => f.Random.String(15));

        return new Faker<Control>()
            .RuleFor(b => b.Name, f => f.Company.CompanyName())
            .RuleFor(b => b.Topic, f => $"/{f.Random.String(0, 10)}")
            .RuleFor(b => b.Icon, iconFaker.Generate())
            .Generate();
    }
}
