using System.Text;
using Client.Models.Models.DTO;
using Infrastructure.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Parsers;

public class CsvProjectParser(ITagRepository tagRepository) : IProjectTableParser
{
    private const int ExpectedFieldCount = 16;

    public async Task<List<FullProjectInfo>> ParseTableAsync(Stream stream, CancellationToken ct)
    {
        var projects = new List<FullProjectInfo>();
        using var parser = new TextFieldParser(stream, Encoding.UTF8, true);
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
        var fields = parser.ReadFields() ?? [];
        if (fields.Length < ExpectedFieldCount)
            throw new InvalidDataException(
                $"CSV row contains {fields.Length} fields, expected at least {ExpectedFieldCount}.");

        var name = fields[0];
        var rawParticipantFields = fields[1..6];
        var rawYear = fields[6];
        var rawSemester = fields[7];
        var shortDescription = fields[8];
        var rawDescriptionLink = fields[9];
        var rawCastDevLink = fields[10];
        var rawMvpLink = fields[11];
        var rawRoadMapLink = fields[12];
        var rawGitLink = fields[13];
        var rawNonGitLink = fields[14];
        var rawTagsJson = fields[15];

        return new FullProjectInfo
        {
            Name = name,
            Year = int.Parse(rawYear),
            Semester = int.Parse(rawSemester),
            Description = shortDescription,
            ShortDescription = shortDescription,
            TeamMembers = ParseTeamMembers(rawParticipantFields),
            Files = ParseFiles(rawDescriptionLink, rawCastDevLink, rawMvpLink, rawRoadMapLink, rawGitLink,
                rawNonGitLink),
            Tags = await ParseTagsAsync(rawTagsJson, ct) ?? []
        };
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

    private static List<TeamMemberDto> ParseTeamMembers(string[] participantFields)
    {
        return participantFields
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => new TeamMemberDto {UserName = name.Trim()})
            .ToList();
    }

    private static FileDto ParseFiles(
        string? descriptionLink,
        string? custDevLink,
        string? mvpLink,
        string? roadMapLink,
        string? gitLink,
        string? nonGitLink)
    {
        var files = new FileDto
        {
            Description = ParseUri(descriptionLink),
            CustDev = ParseUri(custDevLink),
            Mvp = ParseUri(mvpLink),
            RoadMap = ParseUri(roadMapLink)
        };
        var productLinks = new List<Uri>();

        var gitUri = ParseUri(gitLink);
        if (gitUri is not null)
            productLinks.Add(gitUri);

        var nonGitUri = ParseUri(nonGitLink);
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