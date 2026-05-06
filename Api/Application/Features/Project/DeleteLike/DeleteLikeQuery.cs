using Client.Models.Models.DTO;
using MediatR;

namespace Api.Application.Features.Project.PutLike;

public record DeleteLikeQuery(int Id, UserCookie cookie) : IRequest<bool>;