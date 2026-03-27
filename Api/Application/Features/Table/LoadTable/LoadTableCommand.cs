using MediatR;
namespace Api.Application.Features.Table.LoadTable;

public record LoadTableCommand(IParserTable table);