using Common.Devices.Models;

namespace Common.Devices.Validators;

public class DeviceValidator : BaseValidator<Device>
{
    public DeviceValidator() : base()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(32);

        RuleFor(x => x.Placing)
            .MaximumLength(32);

        RuleFor(x => x.BaseDevicePath)
            .MaximumLength(128);

        RuleFor(x => x.BrokerId)
            .NotEmpty();

        RuleFor(x => x.Icon.Name)
            .NotEmpty();

        RuleFor(x => x.Icon.BackgroundHex)
            .NotEmpty();
    }
}
