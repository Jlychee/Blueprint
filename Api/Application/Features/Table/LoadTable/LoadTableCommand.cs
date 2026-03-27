using Client.Models.Models.DTO;
using MediatR;
namespace Api.Application.Features.Table.LoadTable;

public record LoadTableCommand(IFormFile table): IRequest<List<FullProjectInfo>>;