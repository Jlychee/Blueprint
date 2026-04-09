using Infrastructure.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Infrastructure.Entities.File;


namespace Infrastructure.Db;

public class MetricsContext(DbContextOptions<MetricsContext> options) : DbContext(options)
{
    public DbSet<UserRetentionState> UserRetentionStates { get; set; }
    public DbSet<RetentionByCohort> RetentionByCohorts { get; set; }
    public DbSet<FilteredProjectView> FilteredProjectViews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserRetentionStateConfiguration());
        modelBuilder.ApplyConfiguration(new RetentionByCohortConfiguration());
        modelBuilder.ApplyConfiguration(new FilteredProjectViewConfiguration());
        base.OnModelCreating(modelBuilder);
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
        builder.Property(x => x.FilterSessionId)
            .IsRequired();
        builder.Property(x => x.ProjectId)
            .IsRequired();
        builder.Property(x => x.OpenedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.FilterSessionId);
    }
}
