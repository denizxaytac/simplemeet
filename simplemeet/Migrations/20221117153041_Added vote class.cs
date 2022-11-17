using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace simplemeet.Migrations
{
    public partial class Addedvoteclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Choice = table.Column<bool>(type: "bit", nullable: false),
                    User = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vote_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topic_CreatorId",
                table: "Topic",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_TopicId",
                table: "Vote",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_User_CreatorId",
                table: "Topic",
                column: "CreatorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topic_User_CreatorId",
                table: "Topic");

            migrationBuilder.DropTable(
                name: "Vote");

            migrationBuilder.DropIndex(
                name: "IX_Topic_CreatorId",
                table: "Topic");
        }
    }
}
