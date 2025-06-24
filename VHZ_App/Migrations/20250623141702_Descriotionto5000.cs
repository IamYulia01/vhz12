using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VHZ_App.Migrations
{
    /// <inheritdoc />
    public partial class Descriotionto5000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "description_product",
                table: "Product",
                type: "varchar(5000)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "description_product",
                table: "Product",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(5000)",
                oldUnicode: false,
                oldMaxLength: 500);
        }
    }
}
