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

builder.AddBrokerServices();
builder.AddDeviceServices();
builder.AddControlServices();
builder.AddAuthServices();
builder.AddClientServices();
builder.AddUserServices();
builder.AddApplicationServices();

var build = builder.Build();

var authManager = build.Services.GetService<IAuthenticationManager>();
var clientService = build.Services.GetService<IClientService>();

authManager.ObserveLogout(clientService);

await build.RunAsync();
