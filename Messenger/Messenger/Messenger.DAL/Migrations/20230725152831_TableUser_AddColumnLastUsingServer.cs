using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TableUser_AddColumnLastUsingServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastUsingServer",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUsingServer",
                table: "User");
        }
    }
}
