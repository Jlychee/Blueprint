using Client.Models.Models.DTO;
using Infrastructure.Db;
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
        // TODO: если приходит тег, которого в бд нет, мы его скипаем (я хз как мне добавить тег с учетом того, что мы не знаем, какой у него будет тип)
        return await projectContext.Tags
            .Where(t => tagsNames.Contains(t.Title))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken: ct);
    }
}