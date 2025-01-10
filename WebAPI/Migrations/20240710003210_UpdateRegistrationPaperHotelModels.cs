using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRegistrationPaperHotelModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillPaymentHash",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "HashPaper",
                table: "Papers");

            migrationBuilder.AddColumn<byte[]>(
                name: "BillPayment",
                table: "Registrations",
                type: "BINARY(32)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "FullPaper",
                table: "Papers",
                type: "NVARCHAR(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfPaper",
                table: "Papers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Reservation",
                table: "Hotels",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillPayment",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "FullPaper",
                table: "Papers");

            migrationBuilder.DropColumn(
                name: "TypeOfPaper",
                table: "Papers");

            migrationBuilder.AddColumn<byte[]>(
                name: "BillPaymentHash",
                table: "Registrations",
                type: "BINARY(32)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "HashPaper",
                table: "Papers",
                type: "BINARY(32)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Reservation",
                table: "Hotels",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
