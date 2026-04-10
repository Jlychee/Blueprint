using Infrastructure.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> EnsureDatabaseReadyAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var projectContext = scope.ServiceProvider.GetRequiredService<ProjectContext>();
        await projectContext.Database.MigrateAsync();

        var metricsContext = scope.ServiceProvider.GetRequiredService<MetricsContext>();
        await EnsureMetricsTablesAsync(metricsContext);

        return app;
    }

    private static async Task EnsureMetricsTablesAsync(MetricsContext metricsContext)
    {
        await metricsContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "UserRetentionStates"
            (
                "UserId" uuid NOT NULL,
                "FirstOpen" date NOT NULL,
                "SecondOpen" date NOT NULL,
                "r7D" boolean NOT NULL DEFAULT FALSE,
                "r14D" boolean NOT NULL DEFAULT FALSE,
                "r30D" boolean NOT NULL DEFAULT FALSE,
                CONSTRAINT "PK_UserRetentionStates" PRIMARY KEY ("UserId")
            );
            """);

        await metricsContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "RetentionByCohorts"
            (
                "CohortDate" date NOT NULL,
                "CohortWeek" date NOT NULL,
                "Users" integer NOT NULL,
                "r7D" integer NOT NULL,
                "r14D" integer NOT NULL,
                "r30D" integer NOT NULL,
                CONSTRAINT "PK_RetentionByCohorts" PRIMARY KEY ("CohortDate")
            );
            """);

        await metricsContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "FilteredProjectViews"
            (
                "Id" uuid NOT NULL,
                "FilterSessionId" uuid NOT NULL,
                "ProjectId" integer NOT NULL,
                "OpenedAtUtc" timestamp with time zone NOT NULL,
                CONSTRAINT "PK_FilteredProjectViews" PRIMARY KEY ("Id")
            );
            """);

        await metricsContext.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS "IX_FilteredProjectViews_FilterSessionId"
            ON "FilteredProjectViews" ("FilterSessionId");
            """);
    }
}
