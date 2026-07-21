using System.Globalization;
using System.Text.Json.Serialization;
using FlightStatus.Api.Helpers;
using FlightStatus.Api.Middleware;
using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON to serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// OpenAPI / Swagger documentation
builder.Services.AddOpenApi();

// Register normalisation strategies (each provider owns its own)
builder.Services.AddSingleton<AeroTrackStatusNormaliser>();
builder.Services.AddSingleton<QuickFlightStatusNormaliser>();

// Register services
builder.Services.AddSingleton<ITimeBasedStatusDerivationService, TimeBasedStatusDerivationService>();
builder.Services.AddSingleton<IFlightStatusMergeService, FlightStatusMergeService>();
builder.Services.AddSingleton<IFlightStatusProvider, AeroTrackProvider>();
builder.Services.AddSingleton<IFlightStatusProvider, QuickFlightProvider>();
builder.Services.AddSingleton<IFlightStatusService, FlightStatusService>();

// CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Global exception handling
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseCors("AllowFrontend");

// Swagger UI + OpenAPI document (development only)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Flight Status API v1");
    });
}

// GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}
app.MapGet("/flights/status", async (string? flightNumber, string? date, IFlightStatusService flightStatusService, ILogger<Program> logger) =>
{

    if (!FlightNumberValidator.IsValid(flightNumber))
    {
        return Results.BadRequest(new { error = "FlightNumber is required and must be 2 letters followed by 1-4 digits (e.g., SK1234)" });
    }

    if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
    {
        return Results.BadRequest(new { error = "Date is required and must be in yyyy-MM-dd format" });
    }

    logger.LogInformation("API request: flight={FlightNumber}, date={Date}", flightNumber, date);

    var result = await flightStatusService.GetFlightStatusAsync(flightNumber.Trim().ToUpperInvariant(), parsedDate);
    return Results.Ok(result);
})
.WithName("GetFlightStatus")
.WithDescription("Queries multiple flight status providers and returns a merged, normalised result.");

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
