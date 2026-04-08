using Client.Models.Models.DTO;
using Infrastructure.Db;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagRepository(ProjectContext projectContext) : ITagRepository
{
    public async Task<List<TagGroupDto>> GetGroupedTagsAsync(CancellationToken ct) => await projectContext.Tags
        .GroupBy(x => x.TagType)
        .Select(g => new TagGroupDto
        {
            Type = g.Key.Name,
            Tags = g.Select(t => new TagDto
            {
                Id = t.Id,
                Title = t.Title,
                Icon = t.Icon,
                Color = t.Color,
            }).ToList()
        }).ToListAsync(cancellationToken: ct);

    public async Task<List<int>> GetTagsIdsByNameAsync(List<string?> tagsNames, CancellationToken ct) =>
        await projectContext.Tags
            .Where(t => tagsNames.Contains(t.Title))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken: ct);
}