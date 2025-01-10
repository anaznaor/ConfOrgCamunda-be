using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hotel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConferenceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    PaperSubmissionStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaperSubmissionEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateOfRegistration = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserConfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fullname = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Oib = table.Column<string>(type: "NVARCHAR(11)", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfs", x => x.Id);
                });

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
                name: "GuestLecturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdConference = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestLecturers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestLecturers_Conferences_IdConference",
                        column: x => x.IdConference,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuestLecturers_UserConfs_IdUser",
                        column: x => x.IdUser,
                        principalTable: "UserConfs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramCommittees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdConference = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramCommittees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramCommittees_Conferences_IdConference",
                        column: x => x.IdConference,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramCommittees_UserConfs_IdUser",
                        column: x => x.IdUser,
                        principalTable: "UserConfs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConference = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    TimeOfRegistration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRoomReservation = table.Column<int>(type: "int", nullable: true),
                    BillPaymentHash = table.Column<byte[]>(type: "BINARY(32)", nullable: true),
                    IdPaper = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Conferences_IdConference",
                        column: x => x.IdConference,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_UserConfs_IdUser",
                        column: x => x.IdUser,
                        principalTable: "UserConfs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reservation = table.Column<int>(type: "int", nullable: false),
                    IdRegistration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.IdRoom);
                    table.ForeignKey(
                        name: "FK_Hotels_Registrations_IdRegistration",
                        column: x => x.IdRegistration,
                        principalTable: "Registrations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Papers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRegistration = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Abstract = table.Column<string>(type: "NVARCHAR(500)", nullable: false),
                    HashPaper = table.Column<byte[]>(type: "BINARY(32)", nullable: true),
                    Decision = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Papers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Papers_Registrations_IdRegistration",
                        column: x => x.IdRegistration,
                        principalTable: "Registrations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaper = table.Column<int>(type: "int", nullable: false),
                    IdReviewer = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    HashReview = table.Column<byte[]>(type: "BINARY(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Papers_IdPaper",
                        column: x => x.IdPaper,
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_ProgramCommittees_IdReviewer",
                        column: x => x.IdReviewer,
                        principalTable: "ProgramCommittees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSchedule = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TimeStart = table.Column<TimeOnly>(type: "time", nullable: false),
                    TimeEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    IdChairperson = table.Column<int>(type: "int", nullable: false),
                    IdLecturer = table.Column<int>(type: "int", nullable: false),
                    IdPaper = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_GuestLecturers_IdConference",
                table: "GuestLecturers",
                column: "IdConference");

            migrationBuilder.CreateIndex(
                name: "IX_GuestLecturers_IdUser",
                table: "GuestLecturers",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_IdRegistration",
                table: "Hotels",
                column: "IdRegistration",
                unique: true,
                filter: "[IdRegistration] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Papers_IdRegistration",
                table: "Papers",
                column: "IdRegistration",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramCommittees_IdConference",
                table: "ProgramCommittees",
                column: "IdConference");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramCommittees_IdUser",
                table: "ProgramCommittees",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_IdConference",
                table: "Registrations",
                column: "IdConference");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_IdUser",
                table: "Registrations",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IdPaper",
                table: "Reviews",
                column: "IdPaper");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IdReviewer",
                table: "Reviews",
                column: "IdReviewer");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestLecturers");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Papers");

            migrationBuilder.DropTable(
                name: "ProgramCommittees");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Conferences");

            migrationBuilder.DropTable(
                name: "UserConfs");
        }
    }
}
