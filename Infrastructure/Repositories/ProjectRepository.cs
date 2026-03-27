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
            .Distinct();

        var existingUsers = projectContext.Users
            .Where(u => usersFromDto.Contains(u.Name))
            .ToDictionary(u => u.Name, u => u.Id);

        var newUsers = usersFromDto
            .Where(u => !existingUsers.ContainsKey(u))
            .Select(userName => new User { Name = userName }).ToList();

        if (newUsers.Count != 0)
        {
            await projectContext.Users.AddRangeAsync(newUsers, ct);
            await projectContext.SaveChangesAsync(ct);

            foreach (var user in newUsers)
                existingUsers[user.Name] = user.Id;
        }

        var entities = projects.Select(dto => new Project
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
                .Select(t => t.Id)
                .Distinct()
                .Select(tagId => new ProjectTag
                {
                    TagId = tagId,
                }).ToList()
        }).ToList();

        //TODO: а ниче тот факт, что пользователей еще в команду загрузить надо?

        await projectContext.AddRangeAsync(entities, ct);
        await projectContext.SaveChangesAsync(ct);
    }

    public Task<FullProjectInfo?> GetFullProjectInfoAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResultDto<ProjectCardDto>> SearchAsync(ProjectCatalogFilter filter, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}