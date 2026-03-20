namespace Client.Models.Models.Entities;

public class Tag(string title, Uri? icon, string? color)
{
    public required string Title { get; set; } = title;
    public Uri? Icon { get; set; } = icon;
    public string? Color { get; set; } = color;
}