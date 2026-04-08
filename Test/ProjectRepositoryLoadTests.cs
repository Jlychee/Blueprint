using Client.Models.Models.DTO;
using Infrastructure.Db;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

[TestFixture]
public class ProjectRepositoryTests
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
    public async Task LoadProjectsAsync_ShouldAddProjectAndTeamMembers()
    {
        var projects = new List<FullProjectInfo>
        {
            new()
            {
                Name = "Vibik",
                Description = "Описание",
                ShortDescription = "Кратко",
                Year = 2025,
                Semester = 1,
                Files = new FileDto(),
                TeamMembers = new List<TeamMemberDto>
                {
                    new() { UserName = "Котов Илья" },
                    new() { UserName = "Толканюк Катя" },
                    new() { UserName = "Кискина Арина" },
                    new() { UserName = "Скворок Артем" }
                },
                Tags = new List<TagDto>() // теги не проверяем
            }
        };

        await repository.LoadProjectsAsync(projects, CancellationToken.None);

        var projectCount = await projectContext.Projects.CountAsync();
        var userCount = await projectContext.Users.CountAsync();
        var teamMemberCount = await projectContext.TeamMembers.CountAsync();

        Assert.Multiple(() =>
        {
            Assert.That(projectCount, Is.EqualTo(1), "Проект должен быть добавлен");
            Assert.That(userCount, Is.EqualTo(4), "Пользователи должны быть добавлены");
            Assert.That(teamMemberCount, Is.EqualTo(4), "TeamMembers должны быть добавлены");
        });
    }

    [Test]
    public async Task LoadProjectsAsync_EmptyList_ShouldNotAddAnything()
    {
        var projects = new List<FullProjectInfo>();

        await repository.LoadProjectsAsync(projects, CancellationToken.None);

        Assert.That(await projectContext.Projects.CountAsync(), Is.EqualTo(0));
        Assert.That(await projectContext.Users.CountAsync(), Is.EqualTo(0));
        Assert.That(await projectContext.TeamMembers.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task LoadProjectsAsync_ProjectWithoutTeamMembers_ShouldAddProjectOnly()
    {
        var projects = new List<FullProjectInfo>
        {
            new()
            {
                Name = "Проект без команды",
                Description = "Описание",
                ShortDescription = "Кратко",
                Year = 2026,
                Semester = 2,
                Files = new FileDto(),
                TeamMembers = new List<TeamMemberDto>(),
                Tags = new List<TagDto>() // теги не проверяем
            }
        };

        await repository.LoadProjectsAsync(projects, CancellationToken.None);

        Assert.That(await projectContext.Projects.CountAsync(), Is.EqualTo(1));
        Assert.That(await projectContext.TeamMembers.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task LoadProjectsAsync_ProjectWithoutFiles_ShouldHandleNullFiles()
    {
        var projects = new List<FullProjectInfo>
        {
            new()
            {
                Name = "Проект без файлов",
                Description = "Описание",
                ShortDescription = "Кратко",
                Year = 2026,
                Semester = 2,
                Files = null,
                TeamMembers = new List<TeamMemberDto>(),
                Tags = new List<TagDto>()
            }
        };

        await repository.LoadProjectsAsync(projects, CancellationToken.None);

        var project = await projectContext.Projects.Include(p => p.File).FirstAsync();
        Assert.That(project.File, Is.Null);
    }

    [Test]
    public async Task LoadProjectsAsync_DuplicateUsers_ShouldNotDuplicateInDb()
    {
        var projects = new List<FullProjectInfo>
        {
            new()
            {
                Name = "Проект 1",
                Description = "Описание",
                ShortDescription = "Кратко",
                Year = 2026,
                Semester = 1,
                Files = new FileDto(),
                TeamMembers = new List<TeamMemberDto> { new() { UserName = "Иван" } },
                Tags = new List<TagDto>()
            },
            new()
            {
                Name = "Проект 2",
                Description = "Описание",
                ShortDescription = "Кратко",
                Year = 2026,
                Semester = 2,
                Files = new FileDto(),
                TeamMembers = new List<TeamMemberDto> { new() { UserName = "Иван" } },
                Tags = new List<TagDto>()
            }
        };

        await repository.LoadProjectsAsync(projects, CancellationToken.None);

        var userCount = await projectContext.Users.CountAsync();
        Assert.That(userCount, Is.EqualTo(1), "Пользователь не должен дублироваться");
    }

    [Test]
    public async Task LoadProjectsAsync_MultipleProjects_ShouldAddAllProjectsAndUsers()
    {
        var projects = new List<FullProjectInfo>
        {
            new()
            {
                Name = "Проект 1",
                Description = "Описание 1",
                ShortDescription = "Кратко 1",
                Year = 2026,
                Semester = 1,
                Files = new FileDto(),
                TeamMembers = new List<TeamMemberDto> { new() { UserName = "Иван" } },
                Tags = new List<TagDto>()
            },
            new()
            {
                Name = "Проект 2",
                Description = "Описание 2",
                ShortDescription = "Кратко 2",
                Year = 2025,
                Semester = 2,
                Files = new FileDto(),
                TeamMembers = new List<TeamMemberDto> { new() { UserName = "Мария" } },
                Tags = new List<TagDto>()
            }
        };

        await repository.LoadProjectsAsync(projects, CancellationToken.None);

        var projectCount = await projectContext.Projects.CountAsync();
        var userCount = await projectContext.Users.CountAsync();
        var teamMemberCount = await projectContext.TeamMembers.CountAsync();

        Assert.Multiple(() =>
        {
            Assert.That(projectCount, Is.EqualTo(2), "Должно быть добавлено 2 проекта");
            Assert.That(userCount, Is.EqualTo(2), "Должно быть добавлено 2 пользователя");
            Assert.That(teamMemberCount, Is.EqualTo(2), "Должно быть добавлено 2 TeamMembers");
        });
    }
}