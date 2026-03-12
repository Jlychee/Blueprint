using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure;

public class ProjectContext(DbContextOptions<ProjectContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
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
            t.HasCheckConstraint("CK_PROJECT_YEAR", "Year >= 2000 AND Year <= 2100");
            t.HasCheckConstraint("CK_PROJECT_SEMESTER", "Semester IN (3,4)");
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