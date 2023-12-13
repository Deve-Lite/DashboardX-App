namespace Presentation.Devices;

public static class DeviceServicesExtensions
{
    public static WebAssemblyHostBuilder AddDeviceServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IFetchDeviceService, FetchDeviceService>();
        builder.Services.AddScoped<IDeviceService, DeviceService>();

        builder.Services.AddSingleton<IUnusedDeviceService, UnusedDeviceService>();

        return builder;
    }
}
