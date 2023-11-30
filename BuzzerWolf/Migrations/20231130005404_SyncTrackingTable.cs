using System;
using BuzzerWolf.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuzzerWolf.Migrations
{
    /// <inheritdoc />
    public partial class SyncTrackingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Finish = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sync",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataTable = table.Column<string>(type: "TEXT", nullable: false),
                    LastSync = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    NextAutoSync = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sync", x => new { x.TeamId, x.DataTable });
                });
            migrationBuilder.InsertData(
                table: "Sync",
                columns: new[] { nameof(Sync.TeamId), nameof(Sync.DataTable), nameof(Sync.LastSync), nameof(Sync.NextAutoSync) },
                values: new object[,] { { -1, "seasons", DateTimeOffset.MinValue, DateTimeOffset.MinValue } }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Sync");
        }
    }
}
