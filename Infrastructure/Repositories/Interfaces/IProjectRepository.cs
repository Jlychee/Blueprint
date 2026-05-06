using Client.Models.Models.DTO;

namespace Infrastructure.Repositories.Interfaces;

public interface IProjectRepository
{
    Task LoadProjectsAsync(List<FullProjectInfo> projects, CancellationToken ct);
    Task<FullProjectInfo?> GetFullProjectInfoAsync(int id);
    Task<PagedResultDto<ProjectCardDto>> SearchAsync(ProjectCatalogFilter filter, CancellationToken ct);
    Task<bool> LikeProjectAsync(int projectId,Guid userId, CancellationToken ct);
    Task<bool> UnlikeProjectAsync(int projectId,Guid userId, CancellationToken ct);
}