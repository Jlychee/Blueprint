using Client.Models.Models.DTO;
using Infrastructure.Parsers.Interfaces;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Table.LoadTable;

public class LoadTableHandler(IProjectTableParser parserTable, IProjectRepository projectRepository)
    : IRequestHandler<LoadTableCommand, List<FullProjectInfo>>
{
    public async Task<List<FullProjectInfo>> Handle(LoadTableCommand request, CancellationToken cancellationToken)
    {
        var table = request.table;
        if (table == null || table.Length == 0) throw new ArgumentException("Table is empty");
        await using var stream = table.OpenReadStream();
        var projects = await parserTable.ParseTableAsync(stream, cancellationToken);
        await projectRepository.LoadProjectsAsync(projects, cancellationToken); //передали список 
        return projects; //вернули загруженные проекты я хззззззз
    }
}