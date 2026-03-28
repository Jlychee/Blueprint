using Infrastructure.Db;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Test;

[TestFixture]
public class TagRepositoryGetTagsIdsTest
{
    private ProjectContext projectContext;
    private TagRepository repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ProjectContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        projectContext = new ProjectContext(options);
        repository = new TagRepository(projectContext);
    }

    [TearDown]
    public void TearDown()
    {
        projectContext.Dispose();
    }

    [Test]
    public async Task GetTagsIdsByNameAsync_ShouldReturnExistingTags()
    {
        projectContext.Tags.AddRange(
            new Tag { Title = "c#" },
            new Tag { Title = "react" }
        );
        await projectContext.SaveChangesAsync();

        var result = await repository.GetTagsIdsByNameAsync(
            new List<string?> { "c#", "react" },
            CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Distinct().Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetTagsIdsByNameAsync_ShouldCreateNewTags()
    {
        var tags = new List<string?> { "new1", "new2" };

        var result = await repository.GetTagsIdsByNameAsync(tags, CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(2));

        var dbTags = await projectContext.Tags.ToListAsync();
        Assert.That(dbTags.Count, Is.EqualTo(2));
        Assert.That(dbTags.Any(t => t.Title == "new1"));
        Assert.That(dbTags.Any(t => t.Title == "new2"));
    }

    [Test]
    public async Task GetTagsIdsByNameAsync_ShouldNotDuplicateExistingTags()
    {
        projectContext.Tags.Add(new Tag { Title = "c#" });
        await projectContext.SaveChangesAsync();

        var result = await repository.GetTagsIdsByNameAsync(
            new List<string?> { "c#", "c#" },
            CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(1));

        var dbTags = await projectContext.Tags.ToListAsync();
        Assert.That(dbTags.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetTagsIdsByNameAsync_ShouldMixExistingAndNewTags()
    {
        projectContext.Tags.Add(new Tag { Title = "existing" });
        await projectContext.SaveChangesAsync();

        var tags = new List<string?> { "existing", "new" };

        var result = await repository.GetTagsIdsByNameAsync(tags, CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(2));

        var dbTags = await projectContext.Tags.ToListAsync();
        Assert.That(dbTags.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetTagsIdsByNameAsync_EmptyList_ShouldReturnEmpty()
    {
        var tags = new List<string?>();

        var result = await repository.GetTagsIdsByNameAsync(tags, CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetTagsIdsByNameAsync_ShouldReturnIdsOfTags()
    {
        projectContext.Tags.AddRange(
            new Tag { Title = "tag1" },
            new Tag { Title = "tag2" }
        );
        await projectContext.SaveChangesAsync();

        var tags = new List<string?> { "tag1", "tag2" };

        var result = await repository.GetTagsIdsByNameAsync(tags, CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(2));

        var dbTags = await projectContext.Tags.ToListAsync();

        foreach (var tag in dbTags)
        {
            Assert.That(result.Contains(tag.Id));
        }
    }
}