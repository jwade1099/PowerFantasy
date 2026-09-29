using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerFantasy.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.AddColumn<double>(
                name: "PointsAgainst",
                table: "Teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0 );

            migrationBuilder.AddColumn<double>(
                name: "PointsFor",
                table: "Teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0 );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropColumn(
                name: "PointsAgainst",
                table: "Teams" );

            migrationBuilder.DropColumn(
                name: "PointsFor",
                table: "Teams" );
        }
    }
}
