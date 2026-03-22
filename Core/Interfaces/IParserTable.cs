using Microsoft.AspNetCore.Http;
using Client.Models.Models.DTO;

namespace Infrastructure.Interfaces;

public interface IParserTable
{
    public Task<List<FullProjectInfo>> ParseTable(IFormFile  table);
}