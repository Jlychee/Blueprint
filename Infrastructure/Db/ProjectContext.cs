using Infrastructure.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Infrastructure.Entities.File;

namespace Infrastructure.Db;

public class ProjectContext(DbContextOptions<ProjectContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProjectTag> ProjectTags { get; set; }
    public DbSet<File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectTagConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new FileConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(p => p.TeamMembers)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DescriptionAi).IsRequired();
        builder.Property(x => x.ShortDescriptionAi).IsRequired();

        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Semester).IsRequired();
        // TODO: я хз, через Constraint не статическую проверку не сделать (я бы хотела, кнч текущий год проверять)
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_PROJECT_YEAR", "\"Year\" >= 2000 AND \"Year\" <= 2100");
            t.HasCheckConstraint("CK_PROJECT_SEMESTER", "\"Semester\" IN (3,4)");
        });
    }
}

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(x => new { x.ProjectId, x.UserId });
        builder.HasOne(x => x.User)
            .WithMany(x => x.TeamMembers)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Project)
            .WithMany(x => x.TeamMembers)
            .HasForeignKey(x => x.ProjectId)
            .IsRequired();

        builder.Property(x => x.Role).IsRequired();
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}

public class ProjectTagConfiguration : IEntityTypeConfiguration<ProjectTag>
{
    public void Configure(EntityTypeBuilder<ProjectTag> builder)
    {
        builder.HasKey(x => new { x.ProjectId, x.TagId });

        builder.HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTags)
            .HasForeignKey(pt => pt.ProjectId)
            .IsRequired();

        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.ProjectTags)
            .HasForeignKey(pt => pt.TagId)
            .IsRequired();
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Color).HasMaxLength(7);

        builder.Property(f => f.Icon)
            .HasConversion(
                v => v.ToString(),
                v => new Uri(v))
            .HasMaxLength(500);
    }
}

public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(x => x.ProjectId);

        builder.Property(f => f.CustDev).HasUriConversion();
        builder.Property(f => f.Description).HasUriConversion();
        builder.Property(f => f.Mvp).HasUriConversion();
        builder.Property(f => f.RoadMap).HasUriConversion();
        builder.Property(f => f.Product).HasUriConversion();
    }
}