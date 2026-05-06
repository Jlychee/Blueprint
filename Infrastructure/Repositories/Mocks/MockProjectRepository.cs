using Client.Models.Models.DTO;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.Mocks;

public class MockProjectRepository : IProjectRepository
{
    public Task LoadProjectsAsync(List<FullProjectInfo> projects, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task<FullProjectInfo?> GetFullProjectInfoAsync(int id, Guid userId)
    {
        var files = new FileDto
        {
            CustDev = new Uri("http://localhost:5000"),
            Description = new Uri("http://localhost:5000"),
            Mvp = new Uri("http://localhost:5000"),
            RoadMap = new Uri("http://localhost:5000"),
            Product = [new Uri("http://localhost:5000"), new Uri("http://localhost:5000")]
        };
        var project = new FullProjectInfo
        {
            Name = "Test Project",
            Description = "Test Description",
            Year = 2020,
            Semester = 2,
            Files = files,
            Id = 1,
            ShortDescription = "Short Description",
            LikeCount = 0,
            IsLiked = false
        };
        return Task.FromResult<FullProjectInfo?>(project);
    }

    public Task<PagedResultDto<ProjectCardDto>> SearchAsync(ProjectCatalogFilter filter, Guid userId, CancellationToken ct)
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

    public Task<bool> LikeProjectAsync(int projectId, Guid userId, DateTime likedAtUtc, CancellationToken ct)
    {
        return Task.FromResult(true);
    }

    public Task<bool> UnlikeProjectAsync(int projectId, Guid userId, CancellationToken ct)
    {
        return Task.FromResult(false);
    }
}
