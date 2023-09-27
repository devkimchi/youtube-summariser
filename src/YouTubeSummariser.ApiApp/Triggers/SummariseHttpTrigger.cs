using System.Net;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using YouTubeSummariser.ApiApp.Models;
using YouTubeSummariser.ApiApp.Services;

namespace YouTubeSummariser.ApiApp.Triggers;

/// <summary>
/// This represents the HTTP trigger entity that summarise YouTube video.
/// </summary>
public class SummariseHttpTrigger
{
    private readonly IOpenAIService _openai;
    private readonly IYouTubeService _youtube;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SummariseHttpTrigger"/> class.
    /// </summary>
    /// <param name="openai"><see cref="IOpenAIService"/> instance.</param>
    /// <param name="youtube"><see cref="IYouTubeService"/> instance.</param>
    /// <param name="loggerFactory"><see cref="ILoggerFactory"/> instance.</param>
    public SummariseHttpTrigger(IOpenAIService openai, IYouTubeService youtube, ILoggerFactory loggerFactory)
    {
        this._openai = openai ?? throw new ArgumentNullException(nameof(openai));
        this._youtube = youtube ?? throw new ArgumentNullException(nameof(youtube));
        this._logger = loggerFactory.CreateLogger<SummariseHttpTrigger>();
    }

    /// <summary>
    /// Invokes the HTTP trigger to summarise YouTube video.
    /// </summary>
    /// <param name="req"><see cref="HttpRequestData"/> instance.</param>
    /// <returns>Returns the YouTube video summary.</returns>
    [Function(nameof(SummariseAsync))]
    public async Task<HttpResponseData> SummariseAsync([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "summarise")] HttpRequestData req)
    {
        this._logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = default(HttpResponseData);
        var payload = await req.ReadFromJsonAsync<SummariseRequestModel>();
        if (payload == null)
        {
            this._logger.LogError("Payload is null.");

            response = req.CreateResponse(HttpStatusCode.BadRequest);
            return response;
        }
        if (string.IsNullOrWhiteSpace(payload.VideoUrl) == true)
        {
            this._logger.LogError("Video URL is null or empty.");

            response = req.CreateResponse(HttpStatusCode.BadRequest);
            return response;
        }
        if (string.IsNullOrWhiteSpace(payload.LanguageCode) == true)
        {
            payload.LanguageCode = "en";
        }

        try
        {
            var transcript = await this._youtube.GetTranscriptAsync(payload.VideoUrl, payload.LanguageCode);
            var completion = await this._openai.GetCompletionsAsync(transcript, payload.LanguageCode);

            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            await response.WriteStringAsync(completion);

            return response;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);

            response = req.CreateResponse(HttpStatusCode.InternalServerError);
            return response;
        }
    }
}
