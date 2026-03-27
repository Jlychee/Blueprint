using System.Text;
using Client.Models.Models.DTO;
using Client.Models.Models.Enums;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;

namespace Core.Parser;

public class CsvProjectParser : IParserTable
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
            projects.Add(ParseProject(parser));

        return projects;
    }

    private static FullProjectInfo ParseProject(TextFieldParser parser)
    {
        var project = new FullProjectInfo();
        var fields = parser.ReadFields();
        project.TeamMembers = fields[1..6]
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => new TeamMemberDto
                { UserName = name, UserId = 1, Role = TeamRole.Backend }).ToList();
        project.Name = fields[0];
        project.Year = int.Parse(fields[6]);
        project.Semester = int.Parse(fields[7]);
        project.ShortDescription = fields[8];


        project.Files = ParseFiles(fields);

        project.Tags = ParseTags(fields);
        return project;
    }

    private static List<TagDto> ParseTags(string[]? fields)
    {
        return new List<TagDto>{new()};
    }


    private static FileDto ParseFiles(string[]? fields)
    {
        var files = new FileDto
        {
            Description = ParseUri(fields[9]),
            CustDev = ParseUri(fields[10]),
            RoadMap = ParseUri(fields[12]),
            Product = ParseUri(fields[13])
        };
        var mvpLinks = new List<Uri>();

        var gitUri = ParseUri(fields[13]);
        if (gitUri is not null)
            mvpLinks.Add(gitUri);

        var nonGitUri = ParseUri(fields[14]);
        if (nonGitUri is not null)
            mvpLinks.Add(nonGitUri);

        files.Mvp = mvpLinks;
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