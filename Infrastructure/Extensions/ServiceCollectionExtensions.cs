using Infrastructure.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var host = builder.Configuration["DB_HOST"];
        var port = builder.Configuration["DB_PORT"];
        var db = builder.Configuration["DB_NAME"];
        var user = builder.Configuration["DB_USER"];
        var pass = builder.Configuration["DB_PASSWORD"];
        
        var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
        var env = builder.Environment;

        builder.Services.AddDbContext<ProjectContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);

            if (env.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        builder.Services.AddDbContext<MetricsContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);

            if (env.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        return builder;
    }
}
