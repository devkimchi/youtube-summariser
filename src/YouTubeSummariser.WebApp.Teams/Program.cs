using YouTubeSummariser.Components;
using YouTubeSummariser.Components.Facade;
using YouTubeSummariser.WebApp.Teams;
using YouTubeSummariser.WebApp.Teams.Interop.TeamsSDK;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var config = builder.Configuration.Get<ConfigOptions>();
builder.Services.AddTeamsFx(config.TeamsFx.Authentication);
builder.Services.AddScoped<MicrosoftTeams>();

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp =>
{
    var http = sp.GetService<HttpClient>();
    var facade = new YouTubeSummariserClient(http) { ReadResponseAsString = true };
    if (!builder.Environment.IsDevelopment())
    {
        var accessor = sp.GetService<IHttpContextAccessor>();
        var baseUrl = $"{accessor.HttpContext.Request.BaseUrl().TrimEnd('/')}/api";
        facade.BaseUrl = baseUrl;
    }

    return facade;
});

//builder.Services.AddScoped<IProgressBarJsInterop, ProgressBarJsInterop>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapControllers();
});

app.Run();

