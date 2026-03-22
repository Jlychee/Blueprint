using Client.Models.Models.DTO;

namespace Infrastructure.Repositories.Interfaces;

public interface ITagRepository
{
    Task<List<TagGroupDto>> GetGroupedTagsAsync(CancellationToken ct);
}