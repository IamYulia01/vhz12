using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VHZ_App.Migrations
{
    /// <inheritdoc />
    public partial class ChangePhoneNumberToVarchar50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "number_phone",
                table: "Contact",
                type: "varchar(50)",
                unicode: false,
                fixedLength: true,
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(11)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 11,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "number_phone",
                table: "Contact",
                type: "char(11)",
                unicode: false,
                fixedLength: true,
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 11,
                oldNullable: true);
        }
    }
}
