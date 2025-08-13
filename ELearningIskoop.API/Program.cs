using ELearningIskoop.API.Versioning;
using ELearningIskoop.Courses.Infrastructure.Extensions;
using ELearningIskoop.Courses.Infrastructure.Seeds;
using ELeraningIskoop.ServiceDefaults.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Aspire Service Defaults (Serilog, OpenTelemetry, Health Checks)
builder.AddServiceDefaults();



// ELearning Settings
var elearningSettings = builder.Configuration.GetRequiredSection<ELearningSettings>(ELearningSettings.SectionName);
builder.Services.Configure<ELearningSettings>(builder.Configuration.GetSection(ELearningSettings.SectionName));

// API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "ELearning Platform API",
        Version = "v1",
        Description = "E-Learning platform with Clean Architecture + DDD + CQRS"
    });
});
// MediatR için Application layer'ý ekle - ÖNEMLÝ!

builder.Services.AddCoursesInfrastructure(builder.Configuration);


builder.Services.AddApiVersioningConfiguration();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aspire default endpoints (health checks, metrics)
app.MapDefaultEndpoints();
//await app.Services.ApplyCourseMigrationAsync();
// Configure pipeline
if (app.Environment.IsDevelopment())
{
    await CoursesDataSeeder.SeedAsync(app.Services);
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ELearning Platform API v1");
        c.RoutePrefix = string.Empty; // Swagger root'ta açýlsýn
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();

// Test endpoint'leri
app.MapGet("/", () => Results.Json(new
{
    Message = "ELearning Platform API is running!",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
}));


app.MapGet("/api/test/shared-domain", () =>
{
    // Shared.Domain test
    var email = ELearningIskoop.Shared.Domain.ValueObjects.Email.Create("test@iskoopelearning.com");
    var name = ELearningIskoop.Shared.Domain.ValueObjects.PersonName.Create("Murat", "BEGAR");
    var price = ELearningIskoop.Shared.Domain.ValueObjects.Money.CreateTRY(99.99m);
    var duration = ELearningIskoop.Shared.Domain.ValueObjects.Duration.CreateFromHoursAndMinutes(2, 30);

    return Results.Json(new
    {
        Email = email.Value,
        EmailDomain = email.GetDomain(),
        IsInstitutional = email.IsInstitutional(),
        FullName = name.FullName,
        Initials = name.Initials,
        Price = price.GetFormattedAmount(),
        IsFree = price.Amount == 0,
        Duration = duration.GetFormattedDuration(),
        DurationShort = duration.GetShortFormat(),
        IsShortCourse = duration.IsShort
    });
});

app.MapGet("/api/test/logging", () =>
{
    // Serilog test
    Log.Information("Test log message from API endpoint");
    Log.Warning("Test warning message");

    using var userContext = ELeraningIskoop.ServiceDefaults.Logging.LoggingExtensions.EnrichWithUser("test-user", "Student");
    using var timer = ELeraningIskoop.ServiceDefaults.Logging.LoggingExtensions.StartPerformanceTimer("TestOperation");

    Log.Information("Test message with user context");

    return Results.Json(new { Message = "Logging test completed, check logs!" });
});

app.MapGet("/api/test/config", (IOptions<ELearningSettings> settings) =>
{
    return Results.Json(new
    {
        FileUpload = settings.Value.FileUpload,
        Cache = new
        {
            settings.Value.Cache.DefaultDurationMinutes,
            settings.Value.Cache.LongDurationHours
        },
        Security = new
        {
            settings.Value.Security.PasswordMinLength,
            settings.Value.Security.JwtExpirationMinutes
        }
    });
});

app.MapControllers();

Log.Information("ELearning Platform API starting...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
