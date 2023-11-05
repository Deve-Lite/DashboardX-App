namespace Presentation.Clients;

public static class ClientServicesExtensions
{
    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IClientManager, ClientManager>();

        return builder;
    }
}
