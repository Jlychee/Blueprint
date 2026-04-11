using System.Text.Json;
using Infrastructure.Entities;
using File = System.IO.File;

namespace Infrastructure.Db.Seed;

public class TagTypeSeeder
{
    public static void Seed(ProjectContext context)
    {
        if (!context.TagTypes.Any())
        {
            var path = "/etc/Seed/SeedData/tagTypes.json";

            var json = File.ReadAllText(path);
            var tagTypes = JsonSerializer.Deserialize<List<TagType>>(json) ?? [];
            
            
            
            context.TagTypes.AddRange(tagTypes);
            context.SaveChanges();
        }
    }
}