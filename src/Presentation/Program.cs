using Presentation;
using Presentation.Auth;
using Presentation.Brokers;
using Presentation.Controls;
using Presentation.Devices;
using Presentation.Users;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

if (builder.Configuration.GetValue<string>("Environment") == "Development")
    builder.Configuration.AddJsonFile("appsettings.development.json");
else
    builder.Configuration.AddJsonFile("appsettings.production.json");

builder.AddBrokerServices();
builder.AddDeviceServices();
builder.AddControlServices();
builder.AddAuthServices();
builder.AddClientServices();
builder.AddUserServices();
builder.AddApplicationServices();

await builder.Build().RunAsync();
