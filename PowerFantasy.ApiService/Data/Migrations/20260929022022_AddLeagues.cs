using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerFantasy.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeeklyPolls_Season_Week",
                table: "WeeklyPolls");

            migrationBuilder.DropIndex(
                name: "IX_Teams_SleeperRosterId",
                table: "Teams");

            migrationBuilder.AddColumn<Guid>(
                name: "LeagueId",
                table: "WeeklyPolls",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LeagueId",
                table: "Teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SleeperLeagueId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CommissionerUserId = table.Column<string>(type: "text", nullable: false),
                    PublicSlug = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyPolls_LeagueId_Season_Week",
                table: "WeeklyPolls",
                columns: new[] { "LeagueId", "Season", "Week" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId_SleeperRosterId",
                table: "Teams",
                columns: new[] { "LeagueId", "SleeperRosterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_PublicSlug",
                table: "Leagues",
                column: "PublicSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_SleeperLeagueId",
                table: "Leagues",
                column: "SleeperLeagueId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyPolls_Leagues_LeagueId",
                table: "WeeklyPolls",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyPolls_Leagues_LeagueId",
                table: "WeeklyPolls");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyPolls_LeagueId_Season_Week",
                table: "WeeklyPolls");

            migrationBuilder.DropIndex(
                name: "IX_Teams_LeagueId_SleeperRosterId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "WeeklyPolls");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "Teams");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyPolls_Season_Week",
                table: "WeeklyPolls",
                columns: new[] { "Season", "Week" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SleeperRosterId",
                table: "Teams",
                column: "SleeperRosterId",
                unique: true);
        }
    }
}
