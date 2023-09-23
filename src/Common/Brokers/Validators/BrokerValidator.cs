using Common.Brokers.Models;

namespace Common.Brokers.Validators;

public class BrokerValidator : BaseValidator<Broker>
{
    public BrokerValidator() : base()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(32);

        RuleFor(x => x.Port)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Server)
            .MinimumLength(1)
            .MaximumLength(256);

        RuleFor(x => x.Username)
            .MaximumLength(32);

        RuleFor(x => x.Password)
            .MaximumLength(32);

        RuleFor(x => x.ClientId)
            .MinimumLength(3)
            .MaximumLength(64);

        RuleFor(x => x.KeepAlive)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
    }
}
