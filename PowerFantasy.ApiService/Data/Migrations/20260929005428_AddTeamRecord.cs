using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerFantasy.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.AddColumn<int>(
                name: "Losses",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0 );

            migrationBuilder.AddColumn<int>(
                name: "Ties",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0 );

            migrationBuilder.AddColumn<int>(
                name: "Wins",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0 );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropColumn(
                name: "Losses",
                table: "Teams" );

            migrationBuilder.DropColumn(
                name: "Ties",
                table: "Teams" );

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "Teams" );
        }
    }
}
