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
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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
        return builder;
    }
}