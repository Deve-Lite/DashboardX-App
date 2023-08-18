using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MQTTnet;
using MudBlazor;
using MudBlazor.Services;
using Presentation;
using Presentation.Services;
using Presentation.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient
{
    Timeout = TimeSpan.FromSeconds(Convert.ToDouble(builder.Configuration.GetValue<string>("Api:MaxRequestTimeSeconds")!)),
    BaseAddress = new Uri(builder.Configuration.GetValue<string>("Api:Url"))
});

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.PopoverOptions.ThrowOnDuplicateProvider = false;
    config.SnackbarConfiguration.VisibleStateDuration = 2000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationStateProvider>();

builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddSingleton<ILoadingService, LoadingService>();
builder.Services.AddSingleton<ITopicService, TopicService>();
builder.Services.AddSingleton<MqttFactory>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

await builder.Build().RunAsync();
