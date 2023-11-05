using Presentation;
using Presentation.Application.Interfaces;
using Presentation.Auth;
using Presentation.Brokers;
using Presentation.Controls;
using Presentation.Devices;
using Presentation.Users;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//TODO: Test environments variables
var isProduction = builder.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Production";

var apiUrl = builder.Configuration.GetValue<string>("Api:Url")!;
if (isProduction)
    apiUrl = builder.Configuration.GetValue<string>("API_URL")!;

builder.Services.AddSingleton(sp => new HttpClient()
{
    Timeout = TimeSpan.FromSeconds(Convert.ToDouble(builder.Configuration.GetValue<string>("Api:MaxRequestTimeSeconds")!)),
    BaseAddress = new Uri(apiUrl),

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

builder.AddBrokerServices();
builder.AddDeviceServices();
builder.AddControlServices();
builder.AddAuthServices();
builder.AddClientServices();
builder.AddUserServices();
builder.AddApplicationServices();

builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddSingleton<MqttFactory>();

builder.Services.AddLogging();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

await builder.Build().RunAsync();
