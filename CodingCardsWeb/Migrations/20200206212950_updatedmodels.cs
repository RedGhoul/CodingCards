using Microsoft.EntityFrameworkCore.Migrations;

namespace CodingCards.Migrations
{
    public partial class updatedmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardCreatorId",
                table: "Cards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardCreatorId",
                table: "Cards",
                column: "CardCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_CardCreatorId",
                table: "Cards",
                column: "CardCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_CardCreatorId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_CardCreatorId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CardCreatorId",
                table: "Cards");
        }
    }
}
