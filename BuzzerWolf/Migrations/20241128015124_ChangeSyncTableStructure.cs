using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuzzerWolf.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSyncTableStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Sync",
                newName: "EntityId");

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "Sync",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Season",
                table: "Sync");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Sync",
                newName: "TeamId");
        }
    }
}
