using Client.Models.Models.DTO;
using Core.Interfaces;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Api.Application.Features.Table.LoadTable;

public class LoadTableHandler(IParserTable parserTable, IProjectRepository projectRepository): IRequestHandler<LoadTableCommand, List<FullProjectInfo>>
{
    public async Task<List<FullProjectInfo>> Handle(LoadTableCommand request, CancellationToken cancellationToken)
    {
        var table = request.table;
        if (table == null || table.Length == 0)
        {
            throw new ArgumentException("Table is empty");
        }
        var projects = await parserTable.ParseTable(table); //передали таблицу в парсер и получили список
        await projectRepository.LoadProjectsAsync(projects, cancellationToken); //передали список 
        return projects;//вернули загруженные проекты я хззззззз
    }
}