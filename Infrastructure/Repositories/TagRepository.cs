using Client.Models.Models.DTO;
using Infrastructure.Db;
using Infrastructure.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagRepository(ProjectContext projectContext) : ITagRepository
{
    public Task<List<TagGroupDto>> GetGroupedTagsAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<List<int>> GetTagsIdsByNameAsync(List<string?> tagsNames, CancellationToken ct)
    {
        var existingTags = await projectContext.Tags
            .Where(t => tagsNames.Contains(t.Title))
            .ToDictionaryAsync(t => t.Title, t => t.Id, cancellationToken: ct);

        var newTags = tagsNames
            .Where(t => !existingTags.ContainsKey(t))
            .Select(t => new Tag { Title = t })
            .ToList();

        if (newTags.Count != 0)
        {
            await projectContext.Tags.AddRangeAsync(newTags, ct);
            await projectContext.SaveChangesAsync(ct);

            foreach (var tag in newTags)
                existingTags[tag.Title] = tag.Id;
        }

        return existingTags.Values.ToList();
    }
}