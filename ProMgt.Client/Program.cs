using ProMgt.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudExtensions.Services;
using MudBlazor.Extensions;
using MudBlazor;
using ProMgt.Client.Infrastructure.Validators;
using FluentValidation;
using ProMgt.Client.Infrastructure.Settings;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServicesWithExtensions(opt =>
{
    opt.PopoverOptions.ThrowOnDuplicateProvider = false;
    opt.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    opt.SnackbarConfiguration.PreventDuplicates = true;
});

builder.Services.AddMudExtensions();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<DateNotInFutureAttribute>();
builder.Services.AddScoped<ProjectService>();
await builder.Build().RunAsync();
