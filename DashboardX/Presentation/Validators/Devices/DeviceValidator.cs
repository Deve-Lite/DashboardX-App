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

        RuleFor(x => x.Placing)
            .MaximumLength(32);

        RuleFor(x => x.BaseDevicePath)
            .MaximumLength(128);

        RuleFor(x => x.BrokerId)
            .NotEmpty();

        RuleFor(x => x.IconBackgroundColor)
            .NotEmpty();
    }
}
