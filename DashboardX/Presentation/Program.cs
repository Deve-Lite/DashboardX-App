using Blazored.LocalStorage;
using Core.Interfaces;
using Infrastructe.Services;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Presentation;
using Presentation.Services;
using Presentation.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    Timeout = TimeSpan.FromSeconds(Convert.ToDouble(builder.Configuration.GetValue<string>("Api:MaxRequestTimeSeconds")!)),
    BaseAddress = new Uri(builder.Configuration.GetValue<string>("Api:Url"))
});

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationStateProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IToastService, ToastService>();



await builder.Build().RunAsync();
