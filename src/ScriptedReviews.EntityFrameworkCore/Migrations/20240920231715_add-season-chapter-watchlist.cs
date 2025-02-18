using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScriptedReviews.Migrations
{
    /// <inheritdoc />
    public partial class addseasonchapterwatchlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cast",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReleaseDate",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WatchlistId",
                table: "AppSeries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Writer",
                table: "AppSeries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AppChapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Writer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppChapters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppSeasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Chapters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieId = table.Column<int>(type: "int", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSeasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSeasons_AppSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "AppSeries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppWatchlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWatchlists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSeries_WatchlistId",
                table: "AppSeries",
                column: "WatchlistId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSeasons_SerieId",
                table: "AppSeasons",
                column: "SerieId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSeries_AppWatchlists_WatchlistId",
                table: "AppSeries",
                column: "WatchlistId",
                principalTable: "AppWatchlists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSeries_AppWatchlists_WatchlistId",
                table: "AppSeries");

            migrationBuilder.DropTable(
                name: "AppChapters");

            migrationBuilder.DropTable(
                name: "AppSeasons");

            migrationBuilder.DropTable(
                name: "AppWatchlists");

            migrationBuilder.DropIndex(
                name: "IX_AppSeries_WatchlistId",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Cast",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "WatchlistId",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "Writer",
                table: "AppSeries");
        }
    }
}
