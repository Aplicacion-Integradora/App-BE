using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScriptedReviews.Migrations
{
    /// <inheritdoc />
    public partial class Added_UserId_To_Watchlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSeries_AppWatchlists_WatchlistId",
                table: "AppSeries");

            migrationBuilder.DropIndex(
                name: "IX_AppSeries_WatchlistId",
                table: "AppSeries");

            migrationBuilder.DropColumn(
                name: "WatchlistId",
                table: "AppSeries");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppWatchlists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppWatchlists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppWatchlists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppWatchlists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AppWatchlists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AppWatchListSeries",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    WatchlistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWatchListSeries", x => new { x.SeriesId, x.WatchlistId });
                    table.ForeignKey(
                        name: "FK_AppWatchListSeries_AppSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "AppSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppWatchListSeries_AppWatchlists_WatchlistId",
                        column: x => x.WatchlistId,
                        principalTable: "AppWatchlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppWatchListSeries_WatchlistId",
                table: "AppWatchListSeries",
                column: "WatchlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppWatchListSeries");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppWatchlists");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppWatchlists");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppWatchlists");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppWatchlists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppWatchlists");

            migrationBuilder.AddColumn<int>(
                name: "WatchlistId",
                table: "AppSeries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppSeries_WatchlistId",
                table: "AppSeries",
                column: "WatchlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSeries_AppWatchlists_WatchlistId",
                table: "AppSeries",
                column: "WatchlistId",
                principalTable: "AppWatchlists",
                principalColumn: "Id");
        }
    }
}
