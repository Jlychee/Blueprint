using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTagTypeAndRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagTypeId",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Mvp",
                table: "Files",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TagTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ProjectId_UserId",
                table: "TeamMembers",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagTypeId",
                table: "Tags",
                column: "TagTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_ProjectId_TagId",
                table: "ProjectTags",
                columns: new[] { "ProjectId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Year_Semester",
                table: "Projects",
                columns: new[] { "Year", "Semester" });

            migrationBuilder.CreateIndex(
                name: "IX_TagTypes_Type",
                table: "TagTypes",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_TagTypes_Type_Priority",
                table: "TagTypes",
                columns: new[] { "Type", "Priority" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TagTypes_TagTypeId",
                table: "Tags",
                column: "TagTypeId",
                principalTable: "TagTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TagTypes_TagTypeId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "TagTypes");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ProjectId_UserId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagTypeId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTags_ProjectId_TagId",
                table: "ProjectTags");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Year_Semester",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TagTypeId",
                table: "Tags");

            migrationBuilder.AlterColumn<string>(
                name: "Mvp",
                table: "Files",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
