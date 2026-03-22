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
            .GroupBy(m => m.UserId)
            .Select(g => new
            {
                Id = g.Key,
                Name = g.First().UserName
            })
            .ToList();

        var userIds = usersFromDto.Select(u => u.Id).ToList();

        var existingUsers = await projectContext.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(ct);

        var existingIds = existingUsers.Select(u => u.Id).ToHashSet();

        var newUsers = usersFromDto
            .Where(u => !existingIds.Contains(u.Id))
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name
            })
            .ToList();

        if (newUsers.Any())
        {
            await projectContext.Users.AddRangeAsync(newUsers, ct);
            await projectContext.SaveChangesAsync(ct);
        }

        var entities = projects.Select(dto => new Project
        {
            Name = dto.Name,
            DescriptionAi = dto.Description,
            ShortDescriptionAi = dto.ShortDescription,
            Year = dto.Year,
            Semester = dto.Semester,

            File = dto.Files == null ? null : new File
            {
                CustDev = dto.Files.CustDev,
                Description = dto.Files.Description,
                Mvp = dto.Files.Mvp,
                RoadMap = dto.Files.RoadMap,
                Product = dto.Files.Product,
            },

            TeamMembers = dto.TeamMembers.Select(m => new TeamMember
            {
                UserId = m.UserId,
                Role = m.Role,
            }).ToList(),

            ProjectTags = dto.Tags
                .Select(t => t.Id)
                .Distinct()
                .Select(tagId => new ProjectTag
                {
                    TagId = tagId,
                }).ToList()
        }).ToList();

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