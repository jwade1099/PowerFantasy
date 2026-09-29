using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerFantasy.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "uuid", nullable: false ),
                    SleeperRosterId = table.Column<int>( type: "integer", nullable: false ),
                    SleeperOwnerId = table.Column<string>( type: "text", nullable: false ),
                    TeamName = table.Column<string>( type: "text", nullable: false ),
                    ManagerName = table.Column<string>( type: "text", nullable: false ),
                    AvatarUrl = table.Column<string>( type: "text", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_Teams", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "WeeklyPolls",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "uuid", nullable: false ),
                    Season = table.Column<string>( type: "text", nullable: false ),
                    Week = table.Column<int>( type: "integer", nullable: false ),
                    CreatedAt = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_WeeklyPolls", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "PollVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>( type: "uuid", nullable: false ),
                    WeeklyPollId = table.Column<Guid>( type: "uuid", nullable: false ),
                    VoterTeamId = table.Column<Guid>( type: "uuid", nullable: false ),
                    RankedTeamIds = table.Column<List<Guid>>( type: "uuid[]", nullable: false ),
                    SubmittedAt = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_PollVotes", x => x.Id );
                    table.ForeignKey(
                        name: "FK_PollVotes_Teams_VoterTeamId",
                        column: x => x.VoterTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                    table.ForeignKey(
                        name: "FK_PollVotes_WeeklyPolls_WeeklyPollId",
                        column: x => x.WeeklyPollId,
                        principalTable: "WeeklyPolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_VoterTeamId",
                table: "PollVotes",
                column: "VoterTeamId" );

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_WeeklyPollId_VoterTeamId",
                table: "PollVotes",
                columns: new[] { "WeeklyPollId", "VoterTeamId" },
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SleeperRosterId",
                table: "Teams",
                column: "SleeperRosterId",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyPolls_Season_Week",
                table: "WeeklyPolls",
                columns: new[] { "Season", "Week" },
                unique: true );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable(
                name: "PollVotes" );

            migrationBuilder.DropTable(
                name: "Teams" );

            migrationBuilder.DropTable(
                name: "WeeklyPolls" );
        }
    }
}
