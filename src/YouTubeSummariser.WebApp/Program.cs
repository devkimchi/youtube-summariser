using YouTubeSummariser.Components.Facade;
using YouTubeSummariser.WebApp.Components;
using YouTubeSummariser.WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddScoped(sp =>
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
    })
    .AddHttpClient()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
