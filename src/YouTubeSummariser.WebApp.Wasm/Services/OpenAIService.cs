using Azure.AI.OpenAI;
using Azure;

using YouTubeSummariser.WebApp.Wasm.Configurations;

namespace YouTubeSummariser.WebApp.Wasm.Services;

public interface IOpenAIService
{
    Task<string> GetCompletionsAsync(string prompt);
}

public class OpenAIService : IOpenAIService
{
    private readonly OpenAISettings _openAISettings;
    private readonly PromptSettings _promptSettings;

    public OpenAIService(OpenAISettings openAISettings, PromptSettings promptSettings)
    {
        this._openAISettings = openAISettings ?? throw new ArgumentNullException(nameof(openAISettings));
        this._promptSettings = promptSettings ?? throw new ArgumentNullException(nameof(promptSettings));
    }

    public async Task<string> GetCompletionsAsync(string prompt)
    {
        var endpoint = new Uri(this._openAISettings.Endpoint);
        var credential = new AzureKeyCredential(this._openAISettings.ApiKey);
        var client = new OpenAIClient(endpoint, credential);

        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages =
                {
                    new ChatMessage(ChatRole.System, this._promptSettings.System),
                    new ChatMessage(ChatRole.User, prompt)
                },
            MaxTokens = this._promptSettings.MaxTokens,
            Temperature = this._promptSettings.Temperature,
        };

        var deploymentId = this._openAISettings.DeploymentId;

        var response = default(string);
        try
        {
            var result = await client.GetChatCompletionsAsync(deploymentId, chatCompletionsOptions);
            response = result.Value.Choices[0].Message.Content;
        }
        catch
        {
            throw;
        }

        return response;
    }
}
