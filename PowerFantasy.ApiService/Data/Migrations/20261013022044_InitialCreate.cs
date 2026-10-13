using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerFantasy.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SleeperLeagueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionerUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicSlug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SleeperRosterId = table.Column<int>(type: "int", nullable: false),
                    SleeperOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Losses = table.Column<int>(type: "int", nullable: false),
                    Ties = table.Column<int>(type: "int", nullable: false),
                    PointsFor = table.Column<double>(type: "float", nullable: false),
                    PointsAgainst = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyPolls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Season = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyPolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyPolls_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeeklyPollId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoterTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RankedTeamIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollVotes_Teams_VoterTeamId",
                        column: x => x.VoterTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PollVotes_WeeklyPolls_WeeklyPollId",
                        column: x => x.WeeklyPollId,
                        principalTable: "WeeklyPolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_VoterTeamId",
                table: "PollVotes",
                column: "VoterTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_WeeklyPollId_VoterTeamId",
                table: "PollVotes",
                columns: new[] { "WeeklyPollId", "VoterTeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId_SleeperRosterId",
                table: "Teams",
                columns: new[] { "LeagueId", "SleeperRosterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyPolls_LeagueId_Season_Week",
                table: "WeeklyPolls",
                columns: new[] { "LeagueId", "Season", "Week" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollVotes");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "WeeklyPolls");

            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}
