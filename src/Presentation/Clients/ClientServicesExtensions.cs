using Presentation.Application.Interfaces;

namespace Presentation.Clients;

public static class ClientServicesExtensions
{
    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IClientManager, ClientManager>();

        builder.Services.AddSingleton<MqttFactory>();

        return builder;
    }

    public static WebAssemblyHost ObserveLogout(this WebAssemblyHost host)
    {
        var authManager = host.Services.GetService<IAuthorizationManager>()!;
        var clientService = host.Services.GetService<IClientService>()!;

        authManager.ObserveLogout(clientService);

        return host;
    }
}
