using Infrastructure.Db;
using Infrastructure.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MetricRepository(MetricsContext metricsContext) : IMetricRepository
{
    private IMetricRepository _metricRepositoryImplementation;

    public async Task RegisterOpenAsync(Guid userId, DateOnly occurredAt, CancellationToken ct)
    {
        if (userId == Guid.Empty)
            return;

        var userRetentionState = await metricsContext.UserRetentionStates
            .SingleOrDefaultAsync(x => x.UserId == userId, ct);

        if (userRetentionState is null)
        {
            await metricsContext.UserRetentionStates.AddAsync(new UserRetentionState
            {
                UserId = userId,
                FirstOpen = occurredAt,
            }, ct);

            await metricsContext.SaveChangesAsync(ct);
            return;
        }

        if (userRetentionState.FirstOpen == occurredAt || userRetentionState.SecondOpen == occurredAt)
            return;

        if (userRetentionState.SecondOpen != default || occurredAt < userRetentionState.FirstOpen)
            return;

        userRetentionState.SecondOpen = occurredAt;
        await metricsContext.SaveChangesAsync(ct);
    }

    public async Task RegisterFilteredProjectViewAsync(Guid userId, Guid filterSessionId, int projectId,bool hasFilter ,DateTime occurredAtUtc, CancellationToken ct)
    {
        await metricsContext.FilteredProjectViews.AddAsync(new FilteredProjectView
        {
            UserId = userId,
            FilterSessionId = filterSessionId,
            ProjectId = projectId,
            HasFilter = hasFilter,
            OpenedAtUtc = occurredAtUtc,
        }, ct);

        await metricsContext.SaveChangesAsync(ct);
    }

    public async Task RebuildOpenCohortsRetentionAsync(CancellationToken ct)
    {
        var userRetentionStates = await metricsContext.UserRetentionStates.ToListAsync(ct);

        foreach (var userRetentionState in userRetentionStates)
        {
            var hasSecondOpen = userRetentionState.SecondOpen != default;
            var retentionDays = hasSecondOpen
                ? userRetentionState.SecondOpen.DayNumber - userRetentionState.FirstOpen.DayNumber
                : int.MaxValue;

            userRetentionState.r7D = hasSecondOpen && retentionDays <= 7;
            userRetentionState.r14D = hasSecondOpen && retentionDays <= 14;
            userRetentionState.r30D = hasSecondOpen && retentionDays <= 30;
        }

        await metricsContext.SaveChangesAsync(ct);

        var retentionByCohorts = userRetentionStates
            .GroupBy(x => x.FirstOpen)
            .Select(g => new RetentionByCohort
            {
                CohortDate = g.Key,
                CohortWeek = GetCohortWeek(g.Key),
                Users = g.Count(),
                r7D = g.Count(x => x.r7D),
                r14D = g.Count(x => x.r14D),
                r30D = g.Count(x => x.r30D),
            })
            .ToList();

        var existedCohorts = await metricsContext.RetentionByCohorts.ToListAsync(ct);
        if (existedCohorts.Count != 0)
            metricsContext.RetentionByCohorts.RemoveRange(existedCohorts);

        if (retentionByCohorts.Count == 0)
        {
            await metricsContext.SaveChangesAsync(ct);
            return;
        }

        await metricsContext.RetentionByCohorts.AddRangeAsync(retentionByCohorts, ct);
        await metricsContext.SaveChangesAsync(ct);
    }

    public async Task RegisterFilteredViewAsync(Guid userId, Guid filterSessionId, int page, DateTime occurredAtUtc,
        CancellationToken ct)
    {
        await metricsContext.FilteredViews.AddAsync(new FilteredView
        {
            UserId = userId,
            FilterSessionId = filterSessionId,
            Page = page,
            OpenedAtUtc = occurredAtUtc,
        }, ct);

        await metricsContext.SaveChangesAsync(ct);
    }

    private static DateOnly GetCohortWeek(DateOnly cohortDate)
    {
        var daysFromMonday = ((int)cohortDate.DayOfWeek + 6) % 7;
        return cohortDate.AddDays(-daysFromMonday);
    }
}
