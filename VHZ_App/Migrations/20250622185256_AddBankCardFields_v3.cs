using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VHZ_App.Migrations
{
    /// <inheritdoc />
    public partial class AddBankCardFields_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Bank_card",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Bank_card",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Bank_card");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Bank_card");
        }
    }
}
