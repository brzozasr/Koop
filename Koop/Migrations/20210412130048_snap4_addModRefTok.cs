using Microsoft.EntityFrameworkCore.Migrations;

namespace Koop.Migrations
{
    public partial class snap4_addModRefTok : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "refresh_tokens",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "refresh_tokens");
        }
    }
}
