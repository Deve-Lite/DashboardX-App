using Bogus;
using Common;
using Common.Brokers.Models;

namespace PresentationTests.Generators;

public class BrokerDtoGenerator
{
    public static BrokerDTO GenerateBrokerDto()
    {
        var iconFaker = new Faker<Icon>()
            .RuleFor(i => i.BackgroundHex, f => f.Internet.Color())
            .RuleFor(i => i.Name, f => f.Random.String(15));

        return new Faker<BrokerDTO>()
            .RuleFor(b => b.Name, f => f.Company.CompanyName())
            .RuleFor(b => b.KeepAlive, f => f.Random.Int(0, 30))
            .RuleFor(b => b.ClientId, f => f.Name.FirstName())
            .RuleFor(b => b.Port, f => f.Random.Int(433, 10000))
            .RuleFor(b => b.Server, f => f.Internet.Url())
            .RuleFor(b => b.IsSSL, true)
            .RuleFor(b => b.Icon, iconFaker.Generate())
            .Generate();
    }

}
