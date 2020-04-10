using Microsoft.EntityFrameworkCore.Migrations;

namespace CodingCards.Migrations
{
    public partial class cardNumberOfViewUserCards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfViews",
                table: "Cards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ApplicationUserCards",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    CardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserCards", x => new { x.ApplicationUserId, x.CardId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserCards_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserCards_CardId",
                table: "ApplicationUserCards",
                column: "CardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserCards");

            migrationBuilder.DropColumn(
                name: "NumberOfViews",
                table: "Cards");
        }
    }
}
