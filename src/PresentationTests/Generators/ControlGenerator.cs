using Bogus;

namespace PresentationTests.Generators;

internal class ControlGenerator
{
    private static ControlDTO? firstDeviceControl1;
    public static ControlDTO FirstDeviceControl1()
    {
        if (firstDeviceControl1 == null)
        {
            firstDeviceControl1 = GenerateControl();
            firstDeviceControl1.DeviceId = "1";
        }
        return firstDeviceControl1;
    }

    private static ControlDTO? firstDeviceControl2;
    public static ControlDTO FirstDeviceControl2()
    {
        if (firstDeviceControl2 == null)
        {
            firstDeviceControl2 = GenerateControl();
            firstDeviceControl2.DeviceId = "1";
        }
        return firstDeviceControl2;
    }

    private static ControlDTO? firstDeviceControl3;
    public static ControlDTO FirstDeviceControl3()
    {
        if (firstDeviceControl3 == null)
        {
            firstDeviceControl3 = GenerateControl();
            firstDeviceControl3.DeviceId = "1";
        }
        return firstDeviceControl3;
    }


    private static ControlDTO? secondDeviceControl1;
    public static ControlDTO SecondDeviceControl1()
    {
        if (secondDeviceControl1 == null)
        {
            secondDeviceControl1 = GenerateControl();
            secondDeviceControl1.DeviceId = "2";
        }
        return secondDeviceControl1;
    }

    private static ControlDTO? secondDeviceControl2;
    public static ControlDTO SecondDeviceControl2()
    {
        if (secondDeviceControl2 == null)
        {
            secondDeviceControl2 = GenerateControl();
            secondDeviceControl2.DeviceId = "2";
        }
        return secondDeviceControl2;
    }

    private static ControlDTO? secondDeviceControl3;
    public static ControlDTO SecondDeviceControl3()
    {
        if (secondDeviceControl3 == null)
        {
            secondDeviceControl3 = GenerateControl();
            secondDeviceControl3.DeviceId = "2";
        }
        return secondDeviceControl3;
    }

    public static ControlDTO GenerateControl()
    {
        var iconFaker = new Faker<Icon>()
            .RuleFor(i => i.BackgroundHex, f => f.Internet.Color())
            .RuleFor(i => i.Name, f => f.Random.String(15));

        return new Faker<ControlDTO>()
            .RuleFor(b => b.Name, f => f.Company.CompanyName())
            .RuleFor(b => b.Topic, f => $"/{f.Random.String(0, 10)}")
            .RuleFor(b => b.Icon, iconFaker.Generate())
            .RuleFor(b => b.Id, Guid.NewGuid().ToString())
            .Generate();
    }
}
