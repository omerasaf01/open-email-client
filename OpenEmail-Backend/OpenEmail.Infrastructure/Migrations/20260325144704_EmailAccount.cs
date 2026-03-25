using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenEmail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmailAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "EmailAccounts");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "EmailAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAccounts_Id",
                table: "EmailAccounts",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailAccounts_Id",
                table: "EmailAccounts");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "EmailAccounts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "EmailAccounts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
