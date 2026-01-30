
using LoanApp.Web.Services;
using Serilog;
using Serilog.Events;
using System.Runtime.InteropServices;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

//Setup Logging
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("AuthOptions"));
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<ITokenService, TokenService>(client =>
{
    var baseUrl = builder.Configuration["LoanAppApi:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Api:BaseUrl is not configured.");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<ILoanAppApiClient, LoanAppApiClient>(client =>
{
    var baseUrl = builder.Configuration["LoanAppApi:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Api:BaseUrl is not configured.");
    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddTransient<BearerTokenHandler>();

var app = builder.Build();
app.Logger.LogInformation(RuntimeInformation.FrameworkDescription);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
