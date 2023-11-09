using Presentation.Application.Interfaces;

namespace Presentation.Application;

public static class ApplicationServicesExtensions
{
    public static WebAssemblyHostBuilder AddApplicationServices(this WebAssemblyHostBuilder builder)
    {

        builder.Services.AddScoped<AuthenticationStateProvider, ApplicationStateProvider>();

        builder.Services.AddBlazoredLocalStorageAsSingleton();
        builder.Services.AddBlazoredSessionStorageAsSingleton();
        builder.Services.AddSingleton<ILoadingService, LoadingService>();

        var requestTime = builder.Configuration.GetValue<string>("Api:MaxRequestTimeSeconds")!;

#if RELEASE
        var baseAdress = builder.Configuration.GetValue<string>("Api:Production:Url")!;
#else
        var baseAdress = builder.Configuration.GetValue<string>("Api:Development:Url")!;
#endif

        builder.Services.AddSingleton(sp => new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(Convert.ToDouble(requestTime)),
            BaseAddress = new Uri(baseAdress),
        });
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.PopoverOptions.ThrowOnDuplicateProvider = false;
            config.SnackbarConfiguration.VisibleStateDuration = 4000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
        builder.Services.AddAuthorizationCore();
        builder.Services.AddLogging();
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        return builder;
    }
}
