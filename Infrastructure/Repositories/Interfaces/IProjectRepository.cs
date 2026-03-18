using Client.Models.Models.DTO;

namespace Infrastructure.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<FullProjectInfo>  GetFullProjectInfoAsync(int id);
    Task<PagedResultDto<ProjectCardDto>> SearchAsync(ProjectCatalogFilter filter, CancellationToken ct);
}