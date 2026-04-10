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

        var context = scope.ServiceProvider.GetRequiredService<ProjectContext>();

        context.Database.Migrate();

        TagTypeSeeder.Seed(context);
        TagSeeder.Seed(context);

        return app;
    }
}