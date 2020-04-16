using Microsoft.EntityFrameworkCore.Migrations;

namespace CodingCards.Migrations
{
    public partial class Added_LangName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LangName",
                table: "Cards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LangName",
                table: "Cards");
        }
    }
}
