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
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:63342")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.InitializeDatabase();
app.UseCors("FrontendDev");
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapControllers();
app.Run();