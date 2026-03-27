using System.Text;
using Client.Models.Models.DTO;
using Infrastructure.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Parsers;

public class CsvProjectParser(ITagRepository tagRepository) : IProjectTableParser
{
    public async Task<List<FullProjectInfo>> ParseTableAsync(Stream stream, CancellationToken ct)
    {
        var projects = new List<FullProjectInfo>();
        using var parser = new TextFieldParser(stream, Encoding.UTF8, detectEncoding: true);
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        if (parser.EndOfData)
            return projects;

        parser.ReadFields();
        while (!parser.EndOfData)
            projects.Add(await ParseProjectAsync(parser, ct));

        return projects;
    }

    private async Task<FullProjectInfo> ParseProjectAsync(TextFieldParser parser, CancellationToken ct)
    {
        var project = new FullProjectInfo();
        var fields = parser.ReadFields() ?? [];

        project.TeamMembers = fields[1..6]
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => new TeamMemberDto { UserName = name.Trim() })
            .ToList();
        project.Name = fields[0];
        project.Year = int.Parse(fields[6]);
        project.Semester = int.Parse(fields[7]);
        project.ShortDescription = fields[8];
        project.Files = ParseFiles(fields);
        project.Tags = await ParseTagsAsync(fields[15], ct) ?? [];

        return project;
    }

    private async Task<List<TagDto>?> ParseTagsAsync(string tags, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(tags))
            return null;

        var obj = JObject.Parse(tags);
        var allTagsNames = obj.Properties()
            .SelectMany(property => property.Value.Values<string>())
            .ToList();

        var allTagsIds = await tagRepository.GetTagsIdsByNameAsync(allTagsNames, ct);
        return allTagsNames
            .Zip(allTagsIds, (name, id) => new TagDto
            {
                Id = id,
                Title = name ?? string.Empty
            })
            .ToList();
    }

    private static FileDto ParseFiles(string[] fields)
    {
        var files = new FileDto
        {
            Description = ParseUri(fields[9]),
            CustDev = ParseUri(fields[10]),
            Mvp = ParseUri(fields[11]),
            RoadMap = ParseUri(fields[12]),
        };
        var productLinks = new List<Uri>();

        var gitUri = ParseUri(fields[13]);
        if (gitUri is not null)
            productLinks.Add(gitUri);

        var nonGitUri = ParseUri(fields[14]);
        if (nonGitUri is not null)
            productLinks.Add(nonGitUri);

        files.Product = productLinks;
        return files;
    }

    private static Uri? ParseUri(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            ? uri
            : null;
    }
}
