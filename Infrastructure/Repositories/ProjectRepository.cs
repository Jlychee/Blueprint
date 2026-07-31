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

        foreach (var project in newProjects)
        {
            await using var transaction = await projectContext.Database.BeginTransactionAsync(ct);

            try
            {
                var entity = new Project
                {
                    Name = project.Name,
                    DescriptionAi = project.Description,
                    ShortDescriptionAi = project.ShortDescription,
                    Year = project.Year,
                    Semester = project.Semester,
                    File = project.Files == null
                        ? null
                        : new File
                        {
                            CustDev = project.Files.CustDev,
                            Description = project.Files.Description,
                            Mvp = project.Files.Mvp,
                            RoadMap = project.Files.RoadMap,
                            Product = project.Files.Product,
                        },
                    TeamMembers = project.TeamMembers.Select(m => new TeamMember
                    {
                        UserId = existingUsers[m.UserName],
                        Role = m.Role
                    }).ToList(),
                    ProjectTags = project.Tags
                        .Where(t => existingTags.ContainsKey(t.Title))
                        .Select(t => new ProjectTag
                        {
                            TagId = existingTags[t.Title]
                        }).ToList(),
                };

                projectContext.Projects.Add(entity);
                await projectContext.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }

    public Task<FullProjectInfo?> GetFullProjectInfoAsync(int id, Guid userId)
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
                }).ToList(),
                LikeCount = project.LikesCount,
                IsLiked = project.Likes.Any(l => l.UserId == userId),
            }).SingleOrDefaultAsync();
    }

    public async Task<PagedResultDto<ProjectCardDto>> SearchAsync(
        ProjectCatalogFilter filter,
        Guid userId,
        CancellationToken ct)
    {
        var query = projectContext.Projects.AsQueryable();
        
        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{filter.Search.Trim()}%"));

        if (filter.Year.HasValue)
            query = query.Where(p => p.Year == filter.Year.Value);

        if (filter.Semester.HasValue)
            query = query.Where(p => p.Semester == filter.Semester);

        if (filter.TeamMemberCount.HasValue)
            query = query.Where(p => p.TeamMembers.Count == filter.TeamMemberCount);

        if (filter.TagIds?.Any() == true)
        {
            query = query.Where(p =>
                p.ProjectTags
                    .Count(pt => filter.TagIds.Contains(pt.TagId)) == filter.TagIds.Count
            );
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.LikesCount)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new ProjectCardDto
            {
                Id = p.Id,
                Name = p.Name,
                ShortDescriptionAi = p.ShortDescriptionAi,
                Tags = p.ProjectTags
                    .Select(pt => new TagDto
                    {
                        Id = pt.TagId,
                        Title = pt.Tag.Title,
                        Icon = pt.Tag.Icon,
                        Color = pt.Tag.Color,
                    }).ToList(),
                LikeCount = p.LikesCount,
                IsLiked = p.Likes.Any(l => l.UserId == userId),
            }).ToListAsync(ct);

        return new PagedResultDto<ProjectCardDto>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize,
        };
    }

    public async Task<bool> LikeProjectAsync(int projectId, Guid userId, DateTime likedAtUtc, CancellationToken ct)
    {
        await using var transaction = await projectContext.Database.BeginTransactionAsync(ct);

        var projectExists = await projectContext.Projects
            .AnyAsync(x => x.Id == projectId, ct);

        if (!projectExists)
        {
            await transaction.CommitAsync(ct);
            return false;
        }

        var exists = await projectContext.Likes
            .AnyAsync(x => x.ProjectId == projectId && x.UserId == userId, ct);

        if (exists)
        {
            await transaction.CommitAsync(ct);
            return true;
        }

        projectContext.Likes.Add(new Like
        {
            ProjectId = projectId,
            UserId = userId,
            LikedAtUtc = likedAtUtc
        });

        try
        {
            await projectContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException) when (!ct.IsCancellationRequested)
        {
            await transaction.RollbackAsync(ct);
            projectContext.ChangeTracker.Clear();

            var likeExists = await projectContext.Likes
                .AnyAsync(x => x.ProjectId == projectId && x.UserId == userId, ct);

            if (likeExists)
                return true;

            projectExists = await projectContext.Projects
                .AnyAsync(x => x.Id == projectId, ct);

            if (!projectExists)
                return false;

            throw;
        }

        await projectContext.Projects
            .Where(x => x.Id == projectId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.LikesCount, x => x.LikesCount + 1), ct);

        await transaction.CommitAsync(ct);

        return true;
    }

    public async Task<bool> UnlikeProjectAsync(int projectId, Guid userId, CancellationToken ct)
    {
        await using var transaction = await projectContext.Database.BeginTransactionAsync(ct);

        var deleted = await projectContext.Likes
            .Where(x => x.ProjectId == projectId && x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken: ct);

        if (deleted == 0)
        {
            var projectExists = await projectContext.Projects
                .AnyAsync(x => x.Id == projectId, ct);

            await transaction.CommitAsync(ct);
            return projectExists;
        }

        await projectContext.Projects
            .Where(x => x.Id == projectId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.LikesCount, x => x.LikesCount > 0 ? x.LikesCount - 1 : 0), ct);

        await transaction.CommitAsync(ct);

        return true;
    }
}
