using System.Text.Json;
using Infrastructure.Entities;
using File = System.IO.File;

namespace Infrastructure.Db.Seed;

public class TagSeeder
{
    public static void Seed(ProjectContext context)
    {
        var path = Path.Combine(
            "..",
            "etc",
            "SeedData",
            "tag.json"
        );


        var json = File.ReadAllText(path);
        var tags = JsonSerializer.Deserialize<List<Tag>>(json) ?? [];

        foreach (var tag in tags)
        {
            var exists = context.Tags.Any(t => t.Title == tag.Title);

            if (!exists)
                context.Tags.Add(tag);
        }

        context.SaveChanges();
    }
}