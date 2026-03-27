using Client.Models.Models.DTO;
using Client.Models.Models.Enums;
using Infrastructure.Db;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Test
{
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
        public async Task LoadProjectsAsync_ShouldAddProjectAndUser()
        {
            var projects = new List<FullProjectInfo>
            {
                new()
                {
                    Id = 1,
                    Name = "Vibik",
                    Description = "Vibik - это супер-пупер имба, отвечаю",
                    ShortDescription =
                        "Приложение с короткими заданиями и картой воспоминаний, которое мотивирует выйти из дома и превратить прогулку в небольшое атмосферное приключение с целью и наградой.",
                    Year = 2025,
                    Semester = 1,
                    Files = new FileDto
                    {
                        CustDev = new Uri("https://buildin.ai/share/462227e8-c6dd-4442-9420-ac6d2ac9e3ba"),
                        Description =
                            new Uri("https://buildin.ai/share/808390e4-5457-402b-9b19-d4d83f3adc5e?code=AKA9HB"),
                        Mvp = new Uri("https://buildin.ai/3fd65c41-6302-4c95-8397-0d265fdd7503"),
                        Product = new List<Uri> { new Uri("https://github.com/Kitiketov/Vibik") }
                    },
                    TeamMembers = new List<TeamMemberDto>
                    {
                        new() { UserName = "Котов Илья" },
                        new() { UserName = "Толканюк Катя" },
                        new() { UserName = "Кискина Арина" },
                        new() { UserName = "Скворок Артем" },
                    },
                    Tags = new List<TagDto>
                    {
                        new() { Id = 1, Title = "c#" },
                        new() { Id = 2, Title = "android" },
                        new() { Id = 3, Title = "jwt" },
                        new() { Id = 4, Title = "asp.net" },
                        new() { Id = 5, Title = "mobile" },
                    }
                }
            };

            await repository.LoadProjectsAsync(projects, CancellationToken.None);

            var projectCount = await projectContext.Projects.CountAsync();
            var userCount = await projectContext.Users.CountAsync();
            var teamMemberCount = await projectContext.TeamMembers.CountAsync();
            Assert.Multiple(() =>
            {
                Assert.That(projectCount, Is.EqualTo(1), "Проект должен быть добавлен");
                Assert.That(userCount, Is.EqualTo(4), "Пользователь должен быть добавлен");
                Assert.That(teamMemberCount, Is.EqualTo(4), "TeamMember должен быть добавлен");
            });
        }

        [Test]
        public async Task LoadProjectsAsync_EmptyList_ShouldNotAddAnything()
        {
            var projects = new List<FullProjectInfo>();
            await repository.LoadProjectsAsync(projects, CancellationToken.None);

            Assert.Multiple(async () =>
            {
                Assert.That(await projectContext.Projects.CountAsync(), Is.EqualTo(0));
                Assert.That(await projectContext.Users.CountAsync(), Is.EqualTo(0));
                Assert.That(await projectContext.TeamMembers.CountAsync(), Is.EqualTo(0));
            });
        }

        [Test]
        public async Task LoadProjectsAsync_ProjectWithoutTeamMembers_ShouldAddProjectOnly()
        {
            var projects = new List<FullProjectInfo>
            {
                new()
                {
                    Id = 1,
                    Name = "Проект без команды",
                    Description = "Описание",
                    ShortDescription = "Кратко",
                    Year = 2026,
                    Semester = 1,
                    Files = new FileDto(),
                    TeamMembers = new List<TeamMemberDto>(),
                    Tags = new List<TagDto>()
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
                    Id = 1,
                    Name = "Проект без файлов",
                    Description = "Описание",
                    ShortDescription = "Кратко",
                    Year = 2026,
                    Semester = 1,
                    Files = null, // null
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
                    Id = 1,
                    Name = "Проект 1",
                    Description = "Описание",
                    ShortDescription = "Кратко",
                    Year = 2026,
                    Semester = 1,
                    Files = new FileDto(),
                    TeamMembers = new List<TeamMemberDto>
                    {
                        new() { UserName = "Иван", Role = TeamRole.Backend }
                    },
                    Tags = new List<TagDto>()
                },
                new()
                {
                    Id = 2,
                    Name = "Проект 2",
                    Description = "Описание",
                    ShortDescription = "Кратко",
                    Year = 2026,
                    Semester = 2,
                    Files = new FileDto(),
                    TeamMembers = new List<TeamMemberDto>
                    {
                        new() { UserName = "Иван" }
                    },
                    Tags = new List<TagDto>()
                }
            };

            await repository.LoadProjectsAsync(projects, CancellationToken.None);

            var userCount = await projectContext.Users.CountAsync();
            Assert.That(userCount, Is.EqualTo(1), "Пользователь не должен дублироваться");
        }

        [Test]
        public async Task LoadProjectsAsync_DuplicateTags_ShouldAddAllUniqueTags()
        {
            var projects = new List<FullProjectInfo>
            {
                new()
                {
                    Id = 1,
                    Name = "Проект 1",
                    Description = "Описание",
                    ShortDescription = "Кратко",
                    Year = 2026,
                    Semester = 1,
                    Files = new FileDto(),
                    TeamMembers = new List<TeamMemberDto>(),
                    Tags = new List<TagDto>
                    {
                        new() { Id = 1, Title = "c#" },
                        new() { Id = 1, Title = "c#" } // дубликат
                    }
                }
            };

            await repository.LoadProjectsAsync(projects, CancellationToken.None);

            var project = await projectContext.Projects.Include(p => p.ProjectTags).FirstAsync();
            Assert.That(project.ProjectTags.Count, Is.EqualTo(1),
                "Теги будут добавлены как есть, проверяем бизнес-логику");
        }

        [Test]
        public async Task LoadProjectsAsync_ExistingUser_ShouldNotDuplicate()
        {
            projectContext.Users.Add(new User { Id = 1, Name = "Иван" });
            await projectContext.SaveChangesAsync();

            var projects = new List<FullProjectInfo>
            {
                new()
                {
                    Id = 1,
                    Name = "Проект",
                    Description = "Описание",
                    ShortDescription = "Кратко",
                    Year = 2026,
                    Semester = 1,
                    Files = new FileDto(),
                    TeamMembers = new List<TeamMemberDto>
                    {
                        new() { UserName = "Иван", Role = TeamRole.Backend }
                    },
                    Tags = new List<TagDto>()
                }
            };

            await repository.LoadProjectsAsync(projects, CancellationToken.None);

            Assert.That(await projectContext.Users.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task LoadProjectsAsync_MultipleProjects_ShouldAddAllProjects()
        {
            var projects = new List<FullProjectInfo>
            {
                new()
                {
                    Id = 1,
                    Name = "Проект 1",
                    Description = "Описание 1",
                    ShortDescription = "Кратко 1",
                    Year = 2026,
                    Semester = 1,
                    Files = new FileDto(),
                    TeamMembers = new List<TeamMemberDto>
                    {
                        new() { UserName = "Иван" }
                    },
                    Tags = new List<TagDto>
                    {
                        new() { Id = 1, Title = "c#" }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "Проект 2",
                    Description = "Описание 2",
                    ShortDescription = "Кратко 2",
                    Year = 2025,
                    Semester = 2,
                    Files = new FileDto(),
                    TeamMembers = new List<TeamMemberDto>
                    {
                        new() { UserName = "Мария" }
                    },
                    Tags = new List<TagDto>
                    {
                        new() { Id = 2, Title = "react" }
                    }
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
                Assert.That(teamMemberCount, Is.EqualTo(2), "Должно быть добавлено 2 TeamMember");
            });
        }
    }
}