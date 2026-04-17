using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations.Metrics
{
    /// <inheritdoc />
    public partial class CreatedIdForFilteredProjectView_AND_CreatedFilteredViewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilteredProjectViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilterSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    HasFilter = table.Column<bool>(type: "boolean", nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredProjectViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilteredViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilterSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Page = table.Column<int>(type: "integer", nullable: false),
                    Filter = table.Column<string>(type: "jsonb", nullable: true),
                    OpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RetentionByCohorts",
                columns: table => new
                {
                    CohortDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CohortWeek = table.Column<DateOnly>(type: "date", nullable: false),
                    Users = table.Column<int>(type: "integer", nullable: false),
                    r7D = table.Column<int>(type: "integer", nullable: false),
                    r14D = table.Column<int>(type: "integer", nullable: false),
                    r30D = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetentionByCohorts", x => x.CohortDate);
                });

            migrationBuilder.CreateTable(
                name: "UserRetentionStates",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstOpen = table.Column<DateOnly>(type: "date", nullable: false),
                    SecondOpen = table.Column<DateOnly>(type: "date", nullable: false),
                    r7D = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    r14D = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    r30D = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRetentionStates", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilteredProjectViews_FilterSessionId",
                table: "FilteredProjectViews",
                column: "FilterSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_FilteredViews_FilterSessionId",
                table: "FilteredViews",
                column: "FilterSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilteredProjectViews");

            migrationBuilder.DropTable(
                name: "FilteredViews");

            migrationBuilder.DropTable(
                name: "RetentionByCohorts");

            migrationBuilder.DropTable(
                name: "UserRetentionStates");
        }
    }
}
