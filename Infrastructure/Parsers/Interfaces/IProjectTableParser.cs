using Client.Models.Models.DTO;

namespace Infrastructure.Parsers.Interfaces;

public interface IProjectTableParser
{
    Task<List<FullProjectInfo>> ParseTableAsync(Stream stream, CancellationToken ct);
}
