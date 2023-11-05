using Presentation.Application.Interfaces;

namespace Presentation.Application;

public static class ApplicationServicesExtensions
{
    public static WebAssemblyHostBuilder AddApplicationServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddSingleton<ILoadingService, LoadingService>();
        builder.Services.AddScoped<AuthenticationStateProvider, ApplicationStateProvider>();

        return builder;
    }
}
