using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AbstractFullPaperValueType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashReview",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TypeOfPaper",
                table: "Papers");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Reviews",
                type: "NVARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(200)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "BillPayment",
                table: "Registrations",
                type: "VARBINARY(MAX)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BINARY(32)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "FullPaper",
                table: "Papers",
                type: "VARBINARY(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Decision",
                table: "Papers",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Abstract",
                table: "Papers",
                type: "VARBINARY(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(500)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Reviews",
                type: "NVARCHAR(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(500)");

            migrationBuilder.AddColumn<byte[]>(
                name: "HashReview",
                table: "Reviews",
                type: "BINARY(32)",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "BillPayment",
                table: "Registrations",
                type: "BINARY(32)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "VARBINARY(MAX)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullPaper",
                table: "Papers",
                type: "NVARCHAR(2000)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "VARBINARY(MAX)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Decision",
                table: "Papers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Abstract",
                table: "Papers",
                type: "NVARCHAR(500)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(byte[]),
                oldType: "VARBINARY(MAX)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfPaper",
                table: "Papers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
