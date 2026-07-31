using System.Text.Json;
using Client.Models.Models.DTO;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace Infrastructure.Db;

public class MetricsContext(DbContextOptions<MetricsContext> options) : DbContext(options)
{
    public DbSet<UserRetentionState> UserRetentionStates { get; set; }
    public DbSet<RetentionByCohort> RetentionByCohorts { get; set; }
    public DbSet<FilteredProjectView> FilteredProjectViews { get; set; }
    public DbSet<FilteredView> FilteredViews { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserRetentionStateConfiguration());
        modelBuilder.ApplyConfiguration(new RetentionByCohortConfiguration());
        modelBuilder.ApplyConfiguration(new FilteredProjectViewConfiguration());
        modelBuilder.ApplyConfiguration(new FilteredViewConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}



public class FilteredViewConfiguration : IEntityTypeConfiguration<FilteredView>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<FilteredView> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId)
            .IsRequired();
        builder.Property(x => x.FilterSessionId)
            .IsRequired();
        builder.Property(x => x.Page)
            .IsRequired();
        var filterProperty = builder.Property(x => x.Filter)
            .HasConversion(
                filter => SerializeFilter(filter),
                json => DeserializeFilter(json))
            .HasColumnType("jsonb")
            .IsRequired(false);

        filterProperty.Metadata.SetValueComparer(new ValueComparer<ProjectCatalogFilter?>(
            (left, right) => SerializeFilter(left) == SerializeFilter(right),
            filter => SerializeFilter(filter).GetHashCode(),
            filter => DeserializeFilter(SerializeFilter(filter))));

        builder.Property(x => x.OpenedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.FilterSessionId);
    }

    private static string SerializeFilter(ProjectCatalogFilter? filter) =>
        JsonSerializer.Serialize(filter, JsonOptions);

    private static ProjectCatalogFilter? DeserializeFilter(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "null")
            return null;

        return JsonSerializer.Deserialize<ProjectCatalogFilter>(json, JsonOptions);
    }
}

public class RetentionByCohortConfiguration : IEntityTypeConfiguration<RetentionByCohort>
{
    public void Configure(EntityTypeBuilder<RetentionByCohort> builder)
    {
        builder.HasKey(x => x.CohortDate);
        builder.Property(p => p.CohortWeek)
            .IsRequired();
        builder.Property(x => x.Users).IsRequired();

        builder.Property(x => x.r7D).IsRequired();
        builder.Property(x => x.r14D).IsRequired();
        builder.Property(x => x.r30D).IsRequired();
    }
}

public class UserRetentionStateConfiguration : IEntityTypeConfiguration<UserRetentionState>
{
    public void Configure(EntityTypeBuilder<UserRetentionState> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(p => p.FirstOpen)
            .IsRequired();
        builder.Property(x => x.SecondOpen);

        builder.Property(x => x.r7D).HasDefaultValue(false);
        builder.Property(x => x.r14D).HasDefaultValue(false);
        builder.Property(x => x.r30D).HasDefaultValue(false);
    }
}

public class FilteredProjectViewConfiguration : IEntityTypeConfiguration<FilteredProjectView>
{
    public void Configure(EntityTypeBuilder<FilteredProjectView> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId)
            .IsRequired();
        builder.Property(x => x.FilterSessionId)
            .IsRequired();
        builder.Property(x => x.ProjectId)
            .IsRequired();
        builder.Property(x => x.OpenedAtUtc)
            .IsRequired();
        builder.Property(x => x.HasFilter)
            .IsRequired();

        builder.HasIndex(x => x.FilterSessionId);
    }
}