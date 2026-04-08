using Client.Models.Models.DTO;
using Infrastructure.Db;
using Infrastructure.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using File = Infrastructure.Entities.File;

namespace Infrastructure.Repositories;

public class ProjectRepository(ProjectContext projectContext) : IProjectRepository
{
    public async Task LoadProjectsAsync(List<FullProjectInfo> projects, CancellationToken ct)
    {
        var usersFromDto = projects
            .SelectMany(p => p.TeamMembers)
            .Select(p => p.UserName)
            .Distinct()
            .ToList();

        var existingUsers = await projectContext.Users
            .Where(u => usersFromDto.Contains(u.Name))
            .ToDictionaryAsync(u => u.Name, u => u.Id, ct);

        var newUsers = usersFromDto
            .Except(existingUsers.Keys)
            .Select(u => new User { Name = u })
            .ToList();

        if (newUsers.Count != 0)
        {
            await projectContext.Users.AddRangeAsync(newUsers, ct);
            await projectContext.SaveChangesAsync(ct);
            
            foreach (var user in newUsers)
                existingUsers[user.Name] = user.Id;
        }

        var tagsNames = projects
            .SelectMany(p => p.Tags)
            .Select(t => t.Title)
            .Distinct()
            .ToList();

        var existingTags = await projectContext.Tags
            .Where(t => tagsNames.Contains(t.Title))
            .Select(t => new { t.Id, t.Title })
            .ToDictionaryAsync(t => t.Title, t => t.Id, ct);

        // TODO: можно в логи кидать, что при загрузке проекта такие-то теги скипнули, что потом мб добавить
        // var missingTags = tagsNames.Where(t => !existingTags.ContainsKey(t)).ToList();
        //
        // foreach (var missingTag in missingTags)
        // {
        //     Console.WriteLine($"Тега '{missingTag}' нет в таблице Tags. Применяем метод скипа.");
        // }

        var projectNames = projects
            .Select(p => p.Name)
            .Distinct()
            .ToArray();

        var existingProjects = await projectContext.Projects
            .Where(p => projectNames.Contains(p.Name))
            .Select(p => p.Name)
            .ToListAsync(ct);

        var newProjects = projects
            .Where(p => !existingProjects.Contains(p.Name))
            .ToList();

        var entities = newProjects.Select(dto => new Project
        {
            Name = dto.Name,
            DescriptionAi = dto.Description,
            ShortDescriptionAi = dto.ShortDescription,
            Year = dto.Year,
            Semester = dto.Semester,

            File = dto.Files == null
                ? null
                : new File
                {
                    CustDev = dto.Files.CustDev,
                    Description = dto.Files.Description,
                    Mvp = dto.Files.Mvp,
                    RoadMap = dto.Files.RoadMap,
                    Product = dto.Files.Product,
                },

            TeamMembers = dto.TeamMembers.Select(m => new TeamMember
            {
                UserId = existingUsers[m.UserName],
                Role = m.Role ?? null,
            }).ToList(),

            ProjectTags = dto.Tags
                .Select(t => t.Title)
                .Where(title => existingTags.ContainsKey(title))
                .Select(title => new ProjectTag
                {
                    TagId = existingTags[title],
                }).ToList(),
        }).ToList();

        if (entities.Count > 0)
        {
            await projectContext.AddRangeAsync(entities, ct);
            await projectContext.SaveChangesAsync(ct);
        }
    }

    public Task<FullProjectInfo?> GetFullProjectInfoAsync(int id)
    {
        return projectContext.Projects
            .Where(p => p.Id == id)
            .Select(project => new FullProjectInfo
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.DescriptionAi,
                ShortDescription = project.ShortDescriptionAi,
                Year = project.Year,
                Semester = project.Semester,
                Files = new FileDto
                {
                    CustDev = project.File.CustDev,
                    Description = project.File.Description,
                    Mvp = project.File.Mvp,
                    RoadMap = project.File.RoadMap,
                    Product = project.File.Product,
                },
                TeamMembers = project.TeamMembers.Select(m => new TeamMemberDto
                {
                    UserName = m.User.Name,
                    Role = m.Role,
                }).ToList(),
                Tags = project.ProjectTags.Select(t => new TagDto
                {
                    Id = t.TagId,
                    Title = t.Tag.Title,
                    Icon = t.Tag.Icon,
                    Color = t.Tag.Color,
                }).ToList()
            }).SingleOrDefaultAsync();
    }

    public Task<PagedResultDto<ProjectCardDto>> SearchAsync(ProjectCatalogFilter filter, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}