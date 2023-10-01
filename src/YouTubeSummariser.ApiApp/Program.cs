using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Aliencube.YouTubeSubtitlesExtractor;

using Azure;
using Azure.AI.OpenAI;

using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

using YouTubeSummariser.Services;
using YouTubeSummariser.Services.Configurations;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
    //.ConfigureFunctionsWorkerDefaults()
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

        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                OpenApiVersion = OpenApiVersionType.V3,
                Info = new OpenApiInfo()
                {
                    Version = DefaultOpenApiConfigurationOptions.GetOpenApiDocVersion(),
                    Title = DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle(),
                    Description = DefaultOpenApiConfigurationOptions.GetOpenApiDocDescription(),
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("http://opensource.org/licenses/MIT"),
                    }
                },
            };

            return options;
        });
    })

    .Build();

await host.RunAsync();
