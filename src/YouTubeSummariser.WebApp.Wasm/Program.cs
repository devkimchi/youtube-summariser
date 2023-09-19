using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using YouTubeSummariser.WebApp.Wasm;
using YouTubeSummariser.WebApp.Wasm.Configurations;
using YouTubeSummariser.WebApp.Wasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Configuration
//       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true);

var openAISettings = builder.Configuration
                            .GetSection(OpenAISettings.Name)
                            .Get<OpenAISettings>();
builder.Services.AddSingleton(openAISettings);

var promptSettings = builder.Configuration
                            .GetSection(PromptSettings.Name)
                            .Get<PromptSettings>();
builder.Services.AddSingleton(promptSettings);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IOpenAIService, OpenAIService>();

await builder.Build().RunAsync();
