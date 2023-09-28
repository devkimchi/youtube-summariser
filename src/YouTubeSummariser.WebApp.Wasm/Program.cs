using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using YouTubeSummariser.WebApp.Wasm;
using YouTubeSummariser.WebApp.Wasm.Facade;

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

await builder.Build().RunAsync();
