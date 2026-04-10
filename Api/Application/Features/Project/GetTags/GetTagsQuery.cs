using Client.Models.Models.DTO;
using MediatR;

namespace Api.Application.Features.Project.GetTags;

public record GetTagsQuery(): IRequest<List<TagGroupDto>>;