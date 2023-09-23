using Presentation;
using Presentation.Auth;
using Presentation.Brokers;
using Presentation.Clients;
using Presentation.Devices;
using Presentation.Users;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var isProduction = builder.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Production";

var apiUrl = builder.Configuration.GetValue<string>("Api:Url");
if (isProduction)
    apiUrl = builder.Configuration.GetValue<string>("API_URL");

builder.Services.AddSingleton(sp => new HttpClient
{
    Timeout = TimeSpan.FromSeconds(Convert.ToDouble(builder.Configuration.GetValue<string>("Api:MaxRequestTimeSeconds")!)),
    BaseAddress = new Uri(apiUrl)
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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationStateProvider>();

builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddSingleton<ILoadingService, LoadingService>();
builder.Services.AddSingleton<IPrefrenceService, PreferenceService>();
builder.Services.AddSingleton<MqttFactory>();

builder.Services.AddLogging();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

await builder.Build().RunAsync();
