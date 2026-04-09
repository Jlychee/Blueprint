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
    public async Task RebuildOpenCohortsRetentionAsync_ShouldUpdateFlagsAndGroupedMetrics()
    {
        metricsContext.UserRetentionStates.AddRange(
            new UserRetentionState
            {
                UserId = Guid.NewGuid(),
                FirstOpen = new DateOnly(2026, 4, 1),
                SecondOpen = new DateOnly(2026, 4, 8),
            },
            new UserRetentionState
            {
                UserId = Guid.NewGuid(),
                FirstOpen = new DateOnly(2026, 4, 1),
                SecondOpen = new DateOnly(2026, 4, 2),
            });

        await metricsContext.SaveChangesAsync();

        await repository.RebuildOpenCohortsRetentionAsync(CancellationToken.None);

        var states = await metricsContext.UserRetentionStates
            .OrderBy(x => x.SecondOpen)
            .ToListAsync();

        var cohort = await metricsContext.RetentionByCohorts.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(states[0].r7D, Is.False);
            Assert.That(states[1].r7D, Is.True);
            Assert.That(cohort.CohortDate, Is.EqualTo(new DateOnly(2026, 4, 1)));
            Assert.That(cohort.Users, Is.EqualTo(2));
            Assert.That(cohort.r7D, Is.EqualTo(1));
            Assert.That(cohort.r14D, Is.EqualTo(0));
            Assert.That(cohort.r30D, Is.EqualTo(0));
        });
    }
}
