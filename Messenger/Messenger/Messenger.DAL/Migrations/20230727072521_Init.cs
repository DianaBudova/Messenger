using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_User_RecipientId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_User_SenderId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "LastUsingServer",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "LastUsingServerId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_LastUsingServerId",
                table: "User",
                column: "LastUsingServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_User_RecipientId",
                table: "Chat",
                column: "RecipientId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_User_SenderId",
                table: "Chat",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Server_LastUsingServerId",
                table: "User",
                column: "LastUsingServerId",
                principalTable: "Server",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_User_RecipientId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_User_SenderId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Server_LastUsingServerId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_LastUsingServerId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastUsingServerId",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "LastUsingServer",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_User_RecipientId",
                table: "Chat",
                column: "RecipientId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_User_SenderId",
                table: "Chat",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
