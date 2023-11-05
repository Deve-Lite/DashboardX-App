namespace Presentation.Brokers;

public static class BrokerServicesExtensions
{
    public static WebAssemblyHostBuilder AddBrokerServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IFetchBrokerService, FetchBrokerService>();
        builder.Services.AddScoped<IBrokerService, BrokerService>();

        return builder;
    }
}
