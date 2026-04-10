using System.Text.Json;
using Infrastructure.Entities;
using File = System.IO.File;

namespace Infrastructure.Db.Seed;

public class TagSeeder
{
    public static void Seed(ProjectContext context)
    {
        if (!context.Tags.Any())
        {
            var path = "/etc/Seed/SeedData/tag.json";

            var json = File.ReadAllText(path);
            var tags = JsonSerializer.Deserialize<List<Tag>>(json) ?? [];
            
            context.Tags.AddRange(tags);
            context.SaveChanges();
        }
    }
}