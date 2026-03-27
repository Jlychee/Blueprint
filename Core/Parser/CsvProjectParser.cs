using System.Text;
using Client.Models.Models.DTO;
using Client.Models.Models.Enums;
using Core.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;

namespace Core.Parser;

public class CsvProjectParser(ITagRepository tagRepository) : IParserTable
{
    public async Task<List<FullProjectInfo>> ParseTable(IFormFile table)
    {
        var projects = new List<FullProjectInfo>();
        var stream = table.OpenReadStream();
        var parser = new TextFieldParser(stream, Encoding.UTF8, detectEncoding: true);
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        parser.ReadFields();
        while (!parser.EndOfData)
            projects.Add(await ParseProject(parser));

        return projects;
    }

    private async Task<FullProjectInfo> ParseProject(TextFieldParser parser)
    {
        var project = new FullProjectInfo();
        var fields = parser.ReadFields();
        project.TeamMembers = fields[1..6]
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => new TeamMemberDto
                { UserName = name }).ToList();
        project.Name = fields[0];
        project.Year = int.Parse(fields[6]);
        project.Semester = int.Parse(fields[7]);
        project.ShortDescription = fields[8];


        project.Files = ParseFiles(fields);

        project.Tags = await ParseTags(fields[15]);
        return project;
    }

    private async Task<List<TagDto>?> ParseTags(string tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
            return null;
        var obj = JObject.Parse(tags);

        var allTagsNames = obj.Properties()
            .SelectMany(p => p.Value.Values<string>())
            .ToList();
        var allTagsIds = await tagRepository.GetTagsIdsByNameAsync(allTagsNames, CancellationToken.None);
        return allTagsNames
            .Zip(allTagsIds, (name, id) => new TagDto
            {
                Id = id,
                Title = name
            })
            .ToList();
    }


    private static FileDto ParseFiles(string[]? fields)
    {
        var files = new FileDto
        {
            Description = ParseUri(fields[9]),
            CustDev = ParseUri(fields[10]),
            Mvp = ParseUri(fields[11]),
            RoadMap = ParseUri(fields[12]),
        };
        var mvpLinks = new List<Uri>();

        var gitUri = ParseUri(fields[13]);
        if (gitUri is not null)
            mvpLinks.Add(gitUri);

        var nonGitUri = ParseUri(fields[14]);
        if (nonGitUri is not null)
            mvpLinks.Add(nonGitUri);

        files.Product = mvpLinks;
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