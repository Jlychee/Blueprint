using Client.Models.Models.DTO;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.Mocks;

public class MockProjectRepository: IProjectRepository
{
    public Task LoadProjectsAsync(List<FullProjectInfo> projects, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task<FullProjectInfo?> GetFullProjectInfoAsync(int id)
    {
        var project = new FullProjectInfo
        {
            Name = "Test Project",
            Description = "Test Description",
            Year = 2020,
            Semester = 2,
            Files = new FileDto(),
            Id = 1,
            ShortDescription = "Short Description"
        };
        return Task.FromResult<FullProjectInfo?>(project);
    }

    public Task<PagedResultDto<ProjectCardDto>> SearchAsync(ProjectCatalogFilter filter, CancellationToken ct)
    {
        var result = new PagedResultDto<ProjectCardDto>
        {
            Page = 1,
            PageSize = 10,
            TotalCount = 10,
            Items = new List<ProjectCardDto>()
        };
        return Task.FromResult(result);
    }
}