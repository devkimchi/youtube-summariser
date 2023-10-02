using Aliencube.YouTubeSubtitlesExtractor;
using Aliencube.YouTubeSubtitlesExtractor.Abstractions;

using Azure;
using Azure.AI.OpenAI;

using YouTubeSummariser.Services;
using YouTubeSummariser.Services.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var openAISettings = builder.Configuration
                            .GetSection(OpenAISettings.Name)
                            .Get<OpenAISettings>();
builder.Services.AddSingleton(openAISettings);

var promptSettings = builder.Configuration
                            .GetSection(PromptSettings.Name)
                            .Get<PromptSettings>();
builder.Services.AddSingleton(promptSettings);

var endpoint = new Uri(openAISettings.Endpoint);
var credential = new AzureKeyCredential(openAISettings.ApiKey);
var client = new OpenAIClient(endpoint, credential);
builder.Services.AddScoped<OpenAIClient>(_ => client);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IYouTubeVideo, YouTubeVideo>();
builder.Services.AddScoped<IYouTubeService, YouTubeService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwagger(c => c.SerializeAsV2 = true);
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
