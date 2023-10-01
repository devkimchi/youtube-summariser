using System.Net;
using System.Net.Mime;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using YouTubeSummariser.Services;
using YouTubeSummariser.Services.Models;

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
    [OpenApiOperation(operationId: "summarise", tags: new[] { "summary" }, Summary = "Gets the summary from YouTube video", Description = "This gets the summary from a YouTube video.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity(schemeName: "function_key", schemeType: SecuritySchemeType.ApiKey, Name = "x-functions-key", In = OpenApiSecurityLocationType.Header)]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(SummariseRequestModel), Required = true, Description = "The YouTube video information.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Text.Plain, bodyType: typeof(string), Summary = "The YouTube video summary.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid request.", Description = "This indicates the request is invalid.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Internal server error.", Description = "This indicates the server is not working as expected.")]
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
        if (string.IsNullOrWhiteSpace(payload.VideoLanguageCode) == true)
        {
            payload.VideoLanguageCode = "en";
        }
        if (string.IsNullOrWhiteSpace(payload.SummaryLanguageCode) == true)
        {
            payload.SummaryLanguageCode = payload.VideoLanguageCode;
        }

        try
        {
            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var transcript = await this._youtube.GetTranscriptAsync(payload.VideoUrl, payload.VideoLanguageCode);
            if (string.IsNullOrWhiteSpace(transcript) == true)
            {
                var message = "The given YouTube video doesn't provide transcripts.";
                this._logger.LogInformation(message);

                await response.WriteStringAsync(message);

                return response;
            }

            var completion = await this._openai.GetCompletionsAsync(transcript, payload.SummaryLanguageCode);

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
