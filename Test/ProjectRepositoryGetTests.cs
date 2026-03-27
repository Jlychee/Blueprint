using Client.Models.Models.Enums;
using Infrastructure.Db;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using File = Infrastructure.Entities.File;

namespace Test;

[TestFixture]
public class ProjectRepositoryGetTests
{
    private ProjectContext projectContext;
    private ProjectRepository repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ProjectContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        projectContext = new ProjectContext(options);
        repository = new ProjectRepository(projectContext);
    }

    [TearDown]
    public void TearDown()
    {
        projectContext.Dispose();
    }

    [Test]
    public async Task GetFullProjectInfoAsync_ShouldReturnProject_WhenExists()
    {
        var user = new User { Name = "Иван" };
        var tag = new Tag { Title = "c#" };

        projectContext.Users.Add(user);
        projectContext.Tags.Add(tag);
        await projectContext.SaveChangesAsync();

        var project = new Project
        {
            Name = "Test",
            DescriptionAi = "Desc",
            ShortDescriptionAi = "Short",
            Year = 2025,
            Semester = 1,
            File = new File
            {
                CustDev = new Uri("https://test.com"),
                Description = new Uri("https://test.com"),
                Mvp = new Uri("https://test.com"),
                RoadMap = new Uri("https://test.com"),
                Product = new List<Uri> { new("https://test.com") }
            },
            TeamMembers = new List<TeamMember>
            {
                new()
                {
                    User = user
                }
            },
            ProjectTags = new List<ProjectTag>
            {
                new() { Tag = tag }
            }
        };

        projectContext.Projects.Add(project);
        await projectContext.SaveChangesAsync();

        var result = await repository.GetFullProjectInfoAsync(project.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test"));
        Assert.That(result.TeamMembers.Count, Is.EqualTo(1));
        Assert.That(result.Tags.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetFullProjectInfoAsync_ShouldMapFiles()
    {
        var project = new Project
        {
            Name = "Test",
            DescriptionAi = "Desc",
            ShortDescriptionAi = "Short",
            Year = 2025,
            Semester = 1,
            File = new File
            {
                CustDev = new Uri("https://cust"),
                Description = new Uri("https://desc"),
                Mvp = new Uri("https://mvp"),
                RoadMap = new Uri("https://road"),
                Product = new List<Uri> { new("https://prod") }
            }
        };

        projectContext.Projects.Add(project);
        await projectContext.SaveChangesAsync();

        var result = await repository.GetFullProjectInfoAsync(project.Id);

        Assert.That(result.Files, Is.Not.Null);
        Assert.That(result.Files.CustDev.ToString(), Does.Contain("cust"));
        Assert.That(result.Files.Product.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetFullProjectInfoAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await repository.GetFullProjectInfoAsync(999);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetFullProjectInfoAsync_ShouldReturnTeamMembers()
    {
        var user = new User { Name = "Иван" };
        projectContext.Users.Add(user);
        await projectContext.SaveChangesAsync();

        var project = new Project
        {
            Name = "Test",
            DescriptionAi = "Desc",
            ShortDescriptionAi = "Short",
            TeamMembers = new List<TeamMember>
            {
                new() { User = user, Role = TeamRole.Frontend }
            }
        };

        projectContext.Projects.Add(project);
        await projectContext.SaveChangesAsync();

        var result = await repository.GetFullProjectInfoAsync(project.Id);

        Assert.That(result.TeamMembers.Count, Is.EqualTo(1));
        Assert.That(result.TeamMembers[0].UserName, Is.EqualTo("Иван"));
    }

    [Test]
    public async Task GetFullProjectInfoAsync_ShouldReturnTags()
    {
        var tag = new Tag
        {
            Title = "c#",
            Color = "blue"
        };

        projectContext.Tags.Add(tag);
        await projectContext.SaveChangesAsync();

        var project = new Project
        {
            Name = "Test",
            DescriptionAi = "Desc",
            ShortDescriptionAi = "Short",
            ProjectTags = new List<ProjectTag>
            {
                new() { Tag = tag }
            }
        };

        projectContext.Projects.Add(project);
        await projectContext.SaveChangesAsync();

        var result = await repository.GetFullProjectInfoAsync(project.Id);

        Assert.That(result.Tags.Count, Is.EqualTo(1));
        Assert.That(result.Tags[0].Title, Is.EqualTo("c#"));
    }
}