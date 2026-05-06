using Client.Models.Models.DTO;

namespace Infrastructure.Repositories.Interfaces;

public interface IProjectRepository
{
    Task LoadProjectsAsync(List<FullProjectInfo> projects, CancellationToken ct);
    Task<FullProjectInfo?> GetFullProjectInfoAsync(int id, Guid userId);
    Task<PagedResultDto<ProjectCardDto>> SearchAsync(
        ProjectCatalogFilter filter,
        Guid userId,
        CancellationToken ct);
    Task<bool> LikeProjectAsync(int projectId,Guid userId, CancellationToken ct);
    Task<bool> UnlikeProjectAsync(int projectId,Guid userId, CancellationToken ct);
}
