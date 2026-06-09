using Karakatsiya.Constants;
using Karakatsiya.Extensions;
using Karakatsiya.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBusinessServices();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy(AppConstants.Shared.GEO_RATE_LIMITER_POLICY, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 2,
                QueueLimit = 0,
                Window = TimeSpan.FromSeconds(1)
            }));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var supportedCultures = new[] { "en", "ru", "uk" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("uk")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(AppConstants.Shared.CORS_POLICY_NAME);

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await Karakatsiya.Data.DatabaseSeeder.SeedAsync(app.Services);

app.Run();