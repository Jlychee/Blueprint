using System.Reflection;
using Client.Models.Models.Configs;
using Infrastructure.Parsers;
using Infrastructure.Parsers.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;

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
