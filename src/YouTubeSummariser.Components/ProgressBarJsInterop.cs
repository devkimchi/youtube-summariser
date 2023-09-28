using Microsoft.JSInterop;

namespace YouTubeSummariser.Components;

/// <summary>
/// This provides interfaces to the <see cref="ProgressBarJsInterop"/> class.
/// </summary>
public interface IProgressBarJsInterop
{
    /// <summary>
    /// Renders the progress bar.
    /// </summary>
    ValueTask RenderAsync();
}

/// <summary>
/// This represents the JavaScript interop class for progress bar.
/// </summary>
public class ProgressBarJsInterop : IProgressBarJsInterop, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _jsor;

    /// <summary>
    /// Initialises a new instance of the <see cref="ProgressBarJsInterop"/> class.
    /// </summary>
    /// <param name="jsr"><see cref="IJSRuntime"/> instance.</param>
    public ProgressBarJsInterop(IJSRuntime jsr)
        => this._jsor = new(() => jsr.InvokeAsync<IJSObjectReference>("import", "./_content/YouTubeSummariser.Components/js/bundle.js").AsTask());

    /// <inheritdoc/>
    public async ValueTask RenderAsync()
    {
        var module = await this._jsor.Value;
        await module.InvokeVoidAsync("YouTube.RenderProgressBar");
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (this._jsor.IsValueCreated)
        {
            var module = await this._jsor.Value;
            await module.DisposeAsync();
        }
    }
}
