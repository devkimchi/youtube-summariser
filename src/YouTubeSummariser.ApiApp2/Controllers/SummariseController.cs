using System.Net;
using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

using YouTubeSummariser.Services;
using YouTubeSummariser.Services.Models;

namespace YouTubeSummariser.ApiApp2.Controllers;
[ApiController]
[Route("api")]
public class SummariseController : ControllerBase
{
    private readonly IOpenAIService _openai;
    private readonly IYouTubeService _youtube;
    private readonly ILogger<SummariseController> _logger;

    public SummariseController(IOpenAIService openai, IYouTubeService youtube, ILogger<SummariseController> logger)
    {
        this._openai = openai ?? throw new ArgumentNullException(nameof(openai));
        this._youtube = youtube ?? throw new ArgumentNullException(nameof(youtube));
        this._logger = logger;
    }

    [HttpPost("summarise", Name = "summarise")]
    [Consumes(typeof(SummariseRequestModel), MediaTypeNames.Application.Json)]
    [Produces(typeof(string))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SummariseAsync([FromBody] SummariseRequestModel req)
    {
        this._logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = default(IActionResult);
        var payload = req;
        if (payload == null)
        {
            this._logger.LogError("Payload is null.");

            response = new StatusCodeResult((int)HttpStatusCode.BadRequest);
            return response;
        }
        if (string.IsNullOrWhiteSpace(payload.VideoUrl) == true)
        {
            this._logger.LogError("Video URL is null or empty.");

            response = new StatusCodeResult((int)HttpStatusCode.BadRequest);
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
            var result = new ContentResult() { ContentType = MediaTypeNames.Text.Plain, StatusCode = (int)HttpStatusCode.OK };

            var transcript = await this._youtube.GetTranscriptAsync(payload.VideoUrl, payload.VideoLanguageCode);
            if (string.IsNullOrWhiteSpace(transcript) == true)
            {
                var message = "The given YouTube video doesn't provide transcripts.";
                this._logger.LogInformation(message);

                result.Content = message;
                response = result;

                return response;
            }

            var completion = await this._openai.GetCompletionsAsync(transcript, payload.SummaryLanguageCode);

            result.Content = completion;
            response = result;

            return response;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);

            response = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            return response;
        }
    }
}
