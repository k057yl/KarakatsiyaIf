using Karakatsiya.Constants;
using Karakatsiya.Extensions;
using Karakatsiya.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBusinessServices();

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

app.UseCors(AppConstants.Shared.CORS_POLICY_NAME);

app.UseAuthorization();
app.MapControllers();

await Karakatsiya.Data.DatabaseSeeder.SeedAsync(app.Services);

app.Run();