using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VHZ_App.Migrations
{
    /// <inheritdoc />
    public partial class FixBankCardNumberLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "card_number",
                table: "Bank_card",
                type: "nvarchar(19)",
                maxLength: 19,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldFixedLength: true,
                oldMaxLength: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "card_number",
                table: "Bank_card",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(19)",
                oldMaxLength: 19);
        }
    }
}
