using FluentValidation;
using Shared.Models.Devices;

namespace Presentation.Validators.Devices;

public class DeviceValidator : BaseValidator<Device>
{
    public DeviceValidator() : base()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(32);

        //TODO: Custom rule if field can be empty but not bigger than

        RuleFor(x => x.Placing)
            .MaximumLength(64);

        RuleFor(x => x.BaseDevicePath)
            .MinimumLength(0)
            .MaximumLength(128);

        RuleFor(x => x.BrokerId)
            .NotEmpty();
    }
}
