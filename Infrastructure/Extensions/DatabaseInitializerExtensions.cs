using Infrastructure.Db;
using Infrastructure.Db.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class DatabaseInitializerExtensions
{
    public static WebApplication InitializeDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var projectContext = scope.ServiceProvider.GetRequiredService<ProjectContext>();
        projectContext.Database.Migrate();
        
        var metricsContext = scope.ServiceProvider.GetRequiredService<MetricsContext>();
        metricsContext.Database.Migrate();

        TagTypeSeeder.Seed(projectContext);
        TagSeeder.Seed(projectContext);

        return app;
    }
}