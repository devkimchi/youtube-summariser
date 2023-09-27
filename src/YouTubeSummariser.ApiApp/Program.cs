using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Aliencube.YouTubeSubtitlesExtractor;

using Azure;
using Azure.AI.OpenAI;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YouTubeSummariser.ApiApp.Configurations;
using YouTubeSummariser.ApiApp.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureHostConfiguration(config => config.AddEnvironmentVariables())
    .ConfigureServices(services =>
    {
        var openAISettings = services.BuildServiceProvider()
                                     .GetService<IConfiguration>()
                                     .GetSection(OpenAISettings.Name)
                                     .Get<OpenAISettings>();
        services.AddSingleton(openAISettings);

        var promptSettings = services.BuildServiceProvider()
                                     .GetService<IConfiguration>()
                                     .GetSection(PromptSettings.Name)
                                     .Get<PromptSettings>();
        services.AddSingleton(promptSettings);

        var endpoint = new Uri(openAISettings.Endpoint);
        var credential = new AzureKeyCredential(openAISettings.ApiKey);
        var client = new OpenAIClient(endpoint, credential);
        services.AddScoped<OpenAIClient>(_ => client);

        services.AddHttpClient();
        services.AddScoped<IYouTubeVideo, YouTubeVideo>();
        services.AddScoped<IYouTubeService, YouTubeService>();
        services.AddScoped<IOpenAIService, OpenAIService>();
    })

    .Build();

await host.RunAsync();
