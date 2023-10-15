using Common.Brokers.Models;

namespace Common.Brokers.Validators;

public class BrokerCredentialsValidator : BaseValidator<BrokerCredentialsDTO>
{
    public BrokerCredentialsValidator() : base()
    {
        // Empty or both with data

        RuleFor(x => x.Username)
            .MaximumLength(32)
            .When(x => !string.IsNullOrEmpty(x.Username) || !string.IsNullOrEmpty(x.Password));

        RuleFor(x => x.Password)
            .MaximumLength(32)
            .When(x => !string.IsNullOrEmpty(x.Username) || !string.IsNullOrEmpty(x.Password));
    }
}
