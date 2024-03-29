﻿using Common.Devices.Models;

namespace Common.Controls.Models;

public static class ControlExtensions
{
    public static string GetTopic(this Control control, Device device)
    {
        if (string.IsNullOrEmpty(device.BaseDevicePath))
            return $"{control.Topic}";

        return $"{device.BaseDevicePath}{control.Topic}";
    }
}
