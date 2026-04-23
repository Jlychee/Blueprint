using System.Text.Json;
using Infrastructure.Entities;
using File = System.IO.File;

namespace Infrastructure.Db.Seed;

public class TagTypeSeeder
{
    public static void Seed(ProjectContext context)
    {
        var path = Path.Combine(
            "..",
            "etc",
            "SeedData",
            "tagTypes.json"
        );

        var json = File.ReadAllText(path);
        var tagTypes = JsonSerializer.Deserialize<List<TagType>>(json) ?? [];

        foreach (var tagType in tagTypes)
        {
            var exists = context.TagTypes.Any(t => t.Name == tagType.Name);

            if (!exists)
                context.TagTypes.Add(tagType);
        }

        context.SaveChanges();
    }
}