using Client.Models.Models.DTO;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces;

public interface IParserTable
{
    public Task<List<FullProjectInfo>> ParseTable(IFormFile  table);
}