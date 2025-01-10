using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveScheduleFromDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Schedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConference = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Conferences_IdConference",
                        column: x => x.IdConference,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChairperson = table.Column<int>(type: "int", nullable: false),
                    IdLecturer = table.Column<int>(type: "int", nullable: false),
                    IdPaper = table.Column<int>(type: "int", nullable: false),
                    IdSchedule = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TimeEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    TimeStart = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Papers_IdPaper",
                        column: x => x.IdPaper,
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_ProgramCommittees_IdChairperson",
                        column: x => x.IdChairperson,
                        principalTable: "ProgramCommittees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sessions_Schedules_IdSchedule",
                        column: x => x.IdSchedule,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_UserConfs_IdLecturer",
                        column: x => x.IdLecturer,
                        principalTable: "UserConfs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_IdConference",
                table: "Schedules",
                column: "IdConference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IdChairperson",
                table: "Sessions",
                column: "IdChairperson");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IdLecturer",
                table: "Sessions",
                column: "IdLecturer");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IdPaper",
                table: "Sessions",
                column: "IdPaper",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IdSchedule",
                table: "Sessions",
                column: "IdSchedule");
        }
    }
}
