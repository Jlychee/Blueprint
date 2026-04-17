using Api.Application.Common;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseForwardedHeaders();
app.InitializeDatabase();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseExceptionHandler();

app.MapControllers();
app.Run();