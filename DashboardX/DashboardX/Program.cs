using Blazored.LocalStorage;
using DashboardX;
using DashboardX.Auth;
using DashboardX.Brokers;
using DashboardX.Devices;
using DashboardX.Services;
using DashboardX.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.Configuration.GetValue<string>("API:Url")!) ,
    Timeout = TimeSpan.FromSeconds(Convert.ToDouble(builder.Configuration.GetValue<string>("API:MaxReuestTimeSeconds")!))
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddSingleton<ITopicService, TopicService>();

//TODO add auth

await builder.Build().RunAsync();
