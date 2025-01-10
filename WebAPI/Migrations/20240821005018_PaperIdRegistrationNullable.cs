using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class PaperIdRegistrationNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Papers_IdRegistration",
                table: "Papers");

            migrationBuilder.AlterColumn<int>(
                name: "IdRegistration",
                table: "Papers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Papers_IdRegistration",
                table: "Papers",
                column: "IdRegistration",
                unique: true,
                filter: "[IdRegistration] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Papers_IdRegistration",
                table: "Papers");

            migrationBuilder.AlterColumn<int>(
                name: "IdRegistration",
                table: "Papers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Papers_IdRegistration",
                table: "Papers",
                column: "IdRegistration",
                unique: true);
        }
    }
}
