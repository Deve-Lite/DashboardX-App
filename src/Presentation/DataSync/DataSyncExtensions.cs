namespace Presentation.DataSync;

public static class DataSyncExtensions
{
    public static WebAssemblyHostBuilder AddDataSyncServices(this WebAssemblyHostBuilder builder)
    {
        var baseAdress = builder.Configuration.GetValue<string>("Api:Production:Url")!;

        if (builder.HostEnvironment.IsDevelopment())
            baseAdress = builder.Configuration.GetValue<string>("Api:Development:Url")!;

        builder.Services.AddSingleton<ISynchronizer>(sp => new DataSyncService(baseAdress));

        return builder;
    }
}
