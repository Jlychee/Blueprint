using Infrastructure.Db;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Test;

[TestFixture]
public class MetricRepositoryTests
{
    private MetricsContext metricsContext;
    private MetricRepository repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MetricsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        metricsContext = new MetricsContext(options);
        repository = new MetricRepository(metricsContext);
    }

    [TearDown]
    public void TearDown()
    {
        metricsContext.Dispose();
    }

    [Test]
    public async Task RegisterOpenAsync_ShouldCreateUserRetentionState_WhenUserOpensProjectFirstTime()
    {
        var userId = Guid.NewGuid();
        var occurredAt = new DateOnly(2026, 4, 10);

        await repository.RegisterOpenAsync(userId, occurredAt, CancellationToken.None);

        var state = await metricsContext.UserRetentionStates.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(state.UserId, Is.EqualTo(userId));
            Assert.That(state.FirstOpen, Is.EqualTo(occurredAt));
            Assert.That(state.SecondOpen, Is.EqualTo(default(DateOnly)));
        });
    }

    [Test]
    public async Task RegisterOpenAsync_ShouldNotDuplicateOpen_WhenUserEntersSecondTimeOnSameDay()
    {
        var userId = Guid.NewGuid();
        var occurredAt = new DateOnly(2026, 4, 10);

        await repository.RegisterOpenAsync(userId, occurredAt, CancellationToken.None);
        await repository.RegisterOpenAsync(userId, occurredAt, CancellationToken.None);

        var states = await metricsContext.UserRetentionStates.ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(states.Count, Is.EqualTo(1));
            Assert.That(states[0].FirstOpen, Is.EqualTo(occurredAt));
            Assert.That(states[0].SecondOpen, Is.EqualTo(default(DateOnly)));
        });
    }

    [Test]
    public async Task RegisterOpenAsync_ShouldSetSecondOpen_WhenUserReturnsAnotherDay()
    {
        var userId = Guid.NewGuid();

        await repository.RegisterOpenAsync(userId, new DateOnly(2026, 4, 10), CancellationToken.None);
        await repository.RegisterOpenAsync(userId, new DateOnly(2026, 4, 11), CancellationToken.None);

        var state = await metricsContext.UserRetentionStates.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(state.FirstOpen, Is.EqualTo(new DateOnly(2026, 4, 10)));
            Assert.That(state.SecondOpen, Is.EqualTo(new DateOnly(2026, 4, 11)));
        });
    }

    [Test]
    public async Task RegisterOpenAsync_EmptyMetricId_ShouldNotCreateState()
    {
        await repository.RegisterOpenAsync(Guid.Empty, new DateOnly(2026, 4, 10), CancellationToken.None);

        Assert.That(await metricsContext.UserRetentionStates.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task RegisterFilteredProjectViewAsync_ShouldCreateView_WhenFilterSessionIdIsValid()
    {
        var userId = Guid.NewGuid();
        var filterSessionId = Guid.NewGuid();
        var occurredAtUtc = new DateTime(2026, 4, 10, 12, 30, 0, DateTimeKind.Utc);

        await repository.RegisterFilteredProjectViewAsync(userId, filterSessionId, 42, true, occurredAtUtc, CancellationToken.None);

        var view = await metricsContext.FilteredProjectViews.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(view.UserId, Is.EqualTo(userId));
            Assert.That(view.FilterSessionId, Is.EqualTo(filterSessionId));
            Assert.That(view.ProjectId, Is.EqualTo(42));
            Assert.That(view.HasFilter, Is.True);
            Assert.That(view.OpenedAtUtc, Is.EqualTo(occurredAtUtc));
        });
    }

    [Test]
    public async Task RegisterFilteredProjectViewAsync_EmptyFilterSessionId_ShouldCreateUnfilteredView()
    {
        var userId = Guid.NewGuid();
        var occurredAtUtc = new DateTime(2026, 4, 10, 12, 30, 0, DateTimeKind.Utc);

        await repository.RegisterFilteredProjectViewAsync(userId, Guid.Empty, 42, false,
            occurredAtUtc, CancellationToken.None);

        var view = await metricsContext.FilteredProjectViews.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(view.UserId, Is.EqualTo(userId));
            Assert.That(view.FilterSessionId, Is.EqualTo(Guid.Empty));
            Assert.That(view.ProjectId, Is.EqualTo(42));
            Assert.That(view.HasFilter, Is.False);
            Assert.That(view.OpenedAtUtc, Is.EqualTo(occurredAtUtc));
        });
    }

    [Test]
    public async Task RegisterFilteredViewAsync_ShouldCreateView()
    {
        var userId = Guid.NewGuid();
        var filterSessionId = Guid.NewGuid();
        var occurredAtUtc = new DateTime(2026, 4, 10, 13, 0, 0, DateTimeKind.Utc);

        await repository.RegisterFilteredViewAsync(userId, filterSessionId, 3, occurredAtUtc, CancellationToken.None);

        var view = await metricsContext.FilteredViews.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(view.UserId, Is.EqualTo(userId));
            Assert.That(view.FilterSessionId, Is.EqualTo(filterSessionId));
            Assert.That(view.Page, Is.EqualTo(3));
            Assert.That(view.OpenedAtUtc, Is.EqualTo(occurredAtUtc));
            Assert.That(view.Filter, Is.Null);
        });
    }

    [Test]
    public async Task RebuildOpenCohortsRetentionAsync_ShouldUpdateFlagsAndGroupedMetrics()
    {
        var returnedWithinSevenDaysUserId = Guid.NewGuid();
        var returnedWithinFourteenDaysUserId = Guid.NewGuid();
        var didNotReturnUserId = Guid.NewGuid();

        metricsContext.UserRetentionStates.AddRange(
            new UserRetentionState
            {
                UserId = returnedWithinSevenDaysUserId,
                FirstOpen = new DateOnly(2026, 4, 1),
                SecondOpen = new DateOnly(2026, 4, 2),
            },
            new UserRetentionState
            {
                UserId = returnedWithinFourteenDaysUserId,
                FirstOpen = new DateOnly(2026, 4, 1),
                SecondOpen = new DateOnly(2026, 4, 9),
            },
            new UserRetentionState
            {
                UserId = didNotReturnUserId,
                FirstOpen = new DateOnly(2026, 4, 1),
            });

        await metricsContext.SaveChangesAsync();

        await repository.RebuildOpenCohortsRetentionAsync(CancellationToken.None);

        var states = await metricsContext.UserRetentionStates.ToDictionaryAsync(x => x.UserId);

        var cohort = await metricsContext.RetentionByCohorts.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(states[returnedWithinSevenDaysUserId].r7D, Is.True);
            Assert.That(states[returnedWithinSevenDaysUserId].r14D, Is.True);
            Assert.That(states[returnedWithinSevenDaysUserId].r30D, Is.True);

            Assert.That(states[returnedWithinFourteenDaysUserId].r7D, Is.False);
            Assert.That(states[returnedWithinFourteenDaysUserId].r14D, Is.True);
            Assert.That(states[returnedWithinFourteenDaysUserId].r30D, Is.True);

            Assert.That(states[didNotReturnUserId].r7D, Is.False);
            Assert.That(states[didNotReturnUserId].r14D, Is.False);
            Assert.That(states[didNotReturnUserId].r30D, Is.False);

            Assert.That(cohort.CohortDate, Is.EqualTo(new DateOnly(2026, 4, 1)));
            Assert.That(cohort.Users, Is.EqualTo(3));
            Assert.That(cohort.r7D, Is.EqualTo(1));
            Assert.That(cohort.r14D, Is.EqualTo(2));
            Assert.That(cohort.r30D, Is.EqualTo(2));
        });
    }
}
