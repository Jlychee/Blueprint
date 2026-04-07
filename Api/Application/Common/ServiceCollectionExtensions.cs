using System.Reflection;
using Client.Models.Models.Configs;
using Infrastructure.Parsers;
using Infrastructure.Parsers.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Api.Application.Common;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder LoadEnvFiles(this WebApplicationBuilder builder)
    {
        DotNetEnv.Env.Load("../.env");
        DotNetEnv.Env.Load();
        builder.Configuration.AddEnvironmentVariables();

        return builder;
    }
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
    {
        var serviceName = builder.Configuration["Telemetry:ServiceName"];
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            serviceName = builder.Environment.ApplicationName;
        }

        var otlpEndpoint = ResolveOtlpEndpoint(builder.Configuration["Otlp:Endpoint"]);
        var customMeterNames = builder.Configuration.GetSection("Telemetry:MeterNames").Get<string[]>();
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddPrometheusExporter()
                    .AddOtlpExporter(opts => { opts.Endpoint = otlpEndpoint; });

                if (customMeterNames is { Length: > 0 })
                {
                    metrics.AddMeter(customMeterNames);
                }
            })
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(opts => { opts.Endpoint = otlpEndpoint; });
            });

        return builder;
    }

    private static Uri ResolveOtlpEndpoint(string? endpoint)
    {
        const string defaultEndpoint = "http://localhost:4317";

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return new Uri(defaultEndpoint);
        }

        if (Uri.TryCreate(endpoint, UriKind.Absolute, out var parsedEndpoint))
        {
            return parsedEndpoint;
        }

        if (Uri.TryCreate($"http://{endpoint}", UriKind.Absolute, out parsedEndpoint))
        {
            return parsedEndpoint;
        }

        throw new InvalidOperationException(
            $"Invalid OTLP endpoint '{endpoint}'. Use an absolute URI like '{defaultEndpoint}'.");
    }


    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
        {
            var mediatRConfig = builder.Configuration.GetSection("Licenses").Get<MediatRConfig>();
            if (mediatRConfig is not null)
            {
                cfg.LicenseKey = mediatRConfig.LicenseKey;
            }
            cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
        });

        return builder;
    }

    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
        });

        return builder;
    }

    public static WebApplicationBuilder AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();
        builder.Services.AddScoped<IProjectTableParser, CsvProjectParser>();

        return builder;
    }
}
