using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initialize2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Port",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Port",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Port",
                table: "User",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_User_Port",
                table: "User",
                column: "Port",
                unique: true);
        }
    }
}
