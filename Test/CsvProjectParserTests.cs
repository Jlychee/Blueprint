using Infrastructure.Parsers;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories.Mocks;

namespace Test;

[TestFixture]
public class CsvProjectParserTests()
{
    private CsvProjectParser parser;
    private ITagRepository tagRepository;

    [SetUp]
    public void Setup()
    {
        tagRepository = new TagRepositoryMock();
        parser = new CsvProjectParser(tagRepository);
    }
    
    [Test]
    public async Task ParseTable_HeaderOnlyCsv_ReturnsEmptyList()
    {
        using var stream = CreateCsvStream(Header);

        var result = await parser.ParseTableAsync(stream, CancellationToken.None);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ParseTable_CommaSeparatedCsv_MapsBasicProjectFields()
    {
        var csv = """
                  Название продукта,Участник 1 (Фамилия Имя),Участник 2 (Фамилия Имя),Участник 3 (Фамилия Имя),Участник 4 (Фамилия Имя),Участник 5?  (Фамилия Имя),Год,Семестр,Короткое описание,"Описание продукта (текст, ссылка на документ)",ЦА и вопросы CustDev,Описание MVP (ссылка на документ),Дорожная карта проектов,Гиты,Продукты не гит,Код теги
                  LearnTogether,Иван Иванов,Мария Петрова,,,,2024,1,Краткое описание,https://example.com/description,https://example.com/custdev,https://example.com/mvp,https://example.com/roadmap,https://github.com/example/project,,
                  """;
        using var stream = CreateCsvStream(csv);

        var result = await parser.ParseTableAsync(stream, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("LearnTogether"));
            Assert.That(result[0].ShortDescription, Is.EqualTo("Краткое описание"));
            Assert.That(result[0].Year, Is.EqualTo(2024));
            Assert.That(result[0].Semester, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task ParseTable_MultilineQuotedTagsField_DoesNotBreakRecordBoundaries()
    {
        var csv = """
                  Название продукта,Участник 1 (Фамилия Имя),Участник 2 (Фамилия Имя),Участник 3 (Фамилия Имя),Участник 4 (Фамилия Имя),Участник 5?  (Фамилия Имя),Год,Семестр,Короткое описание,"Описание продукта (текст, ссылка на документ)",ЦА и вопросы CustDev,Описание MVP (ссылка на документ),Дорожная карта проектов,Гиты,Продукты не гит,Код теги
                  LearnTogether,Иван Иванов,,,,,2024,1,Описание 1,https://example.com/description-1,,https://example.com/mvp-1,,https://github.com/example/project-1,,"{
                  ""Platform"": [""web"", ""server""],
                  ""Language"": [""c#""]
                  }"
                  GuidesAi,Петр Петров,,,,,2024,2,Описание 2,https://example.com/description-2,,https://example.com/mvp-2,,https://github.com/example/project-2,,
                  """;
        using var stream = CreateCsvStream(csv);

        var result = await parser.ParseTableAsync(stream, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Select(project => project.Name), Is.EqualTo(new[] { "LearnTogether", "GuidesAi" }));
        });
    }

    [Test]
    public async Task ParseTable_TagsJson_MapsTitlesAndIdsFromMockRepository()
    {
        var csv = """
                  Название продукта,Участник 1 (Фамилия Имя),Участник 2 (Фамилия Имя),Участник 3 (Фамилия Имя),Участник 4 (Фамилия Имя),Участник 5?  (Фамилия Имя),Год,Семестр,Короткое описание,"Описание продукта (текст, ссылка на документ)",ЦА и вопросы CustDev,Описание MVP (ссылка на документ),Дорожная карта проектов,Гиты,Продукты не гит,Код теги
                  LearnTogether,Иван Иванов,,,,,2024,1,Описание,https://example.com/description,,https://example.com/mvp,,https://github.com/example/project,,"{
                  ""Platform"": [""web"", ""server""],
                  ""Language"": [""c#""]
                  }"
                  """;
        using var stream = CreateCsvStream(csv);

        var result = await parser.ParseTableAsync(stream, CancellationToken.None);
        var tags = result[0].Tags;

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(tags, Is.Not.Null);
            Assert.That(tags, Has.Count.EqualTo(3));
            Assert.That(tags.Select(tag => tag.Title), Is.EqualTo(new[] { "web", "server", "c#" }));
            Assert.That(tags.Select(tag => tag.Id), Is.EqualTo(new[] { 1, 2, 3 }));
        });
    }

    [Test]
    public async Task ParseTable_BlankOptionalParticipants_DoesNotCreateEmptyTeamMembers()
    {
        var csv = """
                  Название продукта,Участник 1 (Фамилия Имя),Участник 2 (Фамилия Имя),Участник 3 (Фамилия Имя),Участник 4 (Фамилия Имя),Участник 5?  (Фамилия Имя),Год,Семестр,Короткое описание,"Описание продукта (текст, ссылка на документ)",ЦА и вопросы CustDev,Описание MVP (ссылка на документ),Дорожная карта проектов,Гиты,Продукты не гит,Код теги
                  SoloProject,Иван Иванов,,,,,2024,1,Описание,https://example.com/description,,https://example.com/mvp,,https://github.com/example/project,,
                  """;
        using var stream = CreateCsvStream(csv);

        var result = await parser.ParseTableAsync(stream, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].TeamMembers, Has.Count.EqualTo(1));
            Assert.That(result[0].TeamMembers[0].UserName, Is.EqualTo("Иван Иванов"));
        });
    }

    private static MemoryStream CreateCsvStream(string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }

    private const string Header =
        """
        Название продукта,Участник 1 (Фамилия Имя),Участник 2 (Фамилия Имя),Участник 3 (Фамилия Имя),Участник 4 (Фамилия Имя),Участник 5?  (Фамилия Имя),Год,Семестр,Короткое описание,"Описание продукта (текст, ссылка на документ)",ЦА и вопросы CustDev,Описание MVP (ссылка на документ),Дорожная карта проектов,Гиты,Продукты не гит,Код теги
        """;
}
