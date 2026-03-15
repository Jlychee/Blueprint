using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Extensions;

public static class PropertyBuilderExtensions
{
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
}