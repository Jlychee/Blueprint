using Client.Models.Models.DTO;
using MediatR;

namespace Api.Application.Features.Project.DeleteLike;

public record DeleteLikeQuery(int Id, UserCookie cookie) : IRequest<bool>;
