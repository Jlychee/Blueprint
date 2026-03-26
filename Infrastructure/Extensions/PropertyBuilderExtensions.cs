using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Extensions;

public static class PropertyBuilderExtensions
{
    private static string SerializeUriList(List<Uri>? v, int maxLength)
    {
        if (v == null) return null;

        foreach (var el in v)
        {
            var str = el.ToString();
            if (str.Length > maxLength)
                throw new ArgumentException($"Uri too long: {str.Length} > {maxLength}");
        }

        return string.Join(',', v);
    }

    public static PropertyBuilder<Uri?> HasUriConversion(
        this PropertyBuilder<Uri?> builder,
        int maxLength = 500,
        bool isRequired = false)
    {
        return builder.HasConversion(
                v => v == null ? null : v.ToString(),
                v => v == null ? null : new Uri(v))
            .HasMaxLength(maxLength)
            .IsRequired(isRequired);
    }

    public static PropertyBuilder<List<Uri>?> HasUriListConversion(
        this PropertyBuilder<List<Uri>?> builder,
        int maxLength = 500
    )
    {
        return builder.HasConversion(
            v => SerializeUriList(v, maxLength),
            v => v
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(el => new Uri(el)).ToList()
        );
    }
}