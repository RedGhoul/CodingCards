using Microsoft.EntityFrameworkCore.Migrations;

namespace CodingCards.Migrations
{
    public partial class cardNumberOfViewAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfViewAnswers",
                table: "Cards",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfViewAnswers",
                table: "Cards");
        }
    }
}
