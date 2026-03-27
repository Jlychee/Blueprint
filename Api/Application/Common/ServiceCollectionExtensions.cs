using System.Reflection;
using Client.Models.Models.Configs;
using Infrastructure.Interfaces;
using Infrastructure.Mocks;
using Infrastructure.Parsers;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories.Mocks;

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
        // TODO: сюда моки
        builder.Services.AddScoped<IProjectRepository, MockProjectRepository>();
        builder.Services.AddScoped<ITagRepository, TagRepositoryMock>();
        builder.Services.AddScoped<IProjectTableParser, CsvProjectParser>();

        return builder;
    }
}
