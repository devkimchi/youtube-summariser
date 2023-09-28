using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using YouTubeSummariser.Components;
using YouTubeSummariser.Components.Facade;
using YouTubeSummariser.WebApp.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Configuration
//       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp =>
{
    var http = sp.GetService<HttpClient>();
    var facade = new YouTubeSummariserClient(http) { ReadResponseAsString = true };
    if (!builder.HostEnvironment.IsDevelopment())
    {
        var baseUrl = $"{builder.HostEnvironment.BaseAddress.TrimEnd('/')}/api";
        facade.BaseUrl = baseUrl;
    }

    return facade;
});

//builder.Services.AddScoped<IProgressBarJsInterop, ProgressBarJsInterop>();

await builder.Build().RunAsync();
