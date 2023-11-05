namespace Presentation.Controls;

public static class ControlServicesExtensions
{
    public static WebAssemblyHostBuilder AddControlServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IFetchControlService, FetchControlService>();
        builder.Services.AddScoped<IControlService, ControlService>();

        return builder;
    }
}
