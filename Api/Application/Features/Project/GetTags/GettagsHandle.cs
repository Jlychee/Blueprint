using Client.Models.Models.DTO;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Project.GetTags;

public class GetProjectHandle(ITagRepository tagRepository) : IRequestHandler<GetTagsQuery, List<TagGroupDto>>
{
    public async Task<List<TagGroupDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        return await tagRepository.GetGroupedTagsAsync(cancellationToken)
            ?? throw new KeyNotFoundException();
    }
}