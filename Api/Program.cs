using Api.Application.Common;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails(Options => { Options.CustomizeProblemDetails = ProblemDetailsConfig.Configure; });

builder
    .LoadEnvFiles()
    .AddTelemetry()
    .AddSwagger()
    .AddApplicationServices()
    .AddDatabase()
    .AddInfrastructureServices();

var app = builder
  .Build()
  .InitializeDatabase();

// await app.EnsureDatabaseReadyAsync();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("FrontendDev");
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapControllers();
app.Run();

app.UseExceptionHandler();

app.MapControllers();
app.Run();