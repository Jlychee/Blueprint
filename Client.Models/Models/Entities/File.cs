namespace Client.Models.Models.Entities;

public class File(string name, Uri link)
{
    string Name { get; set; } = name;
    Uri Link { get; set; } = link;
}