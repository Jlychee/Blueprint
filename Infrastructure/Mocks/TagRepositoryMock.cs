using Client.Models.Models.DTO;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Mocks;

public class TagRepositoryMock : ITagRepository
{
    public Task<List<TagGroupDto>> GetGroupedTagsAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<int>> GetTagsIdsByNameAsync(List<string> tagsNames, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}