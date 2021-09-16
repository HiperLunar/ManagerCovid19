using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManagerCovid19.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberRegistrationNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "Date", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberRegistrationNumber);
                });

            migrationBuilder.CreateTable(
                name: "HealthRegistration",
                columns: table => new
                {
                    HealthRegistrationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberRegistrationNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RegisterDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HowRUFeeling = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRegistration", x => x.HealthRegistrationID);
                    table.ForeignKey(
                        name: "FK_HealthRegistration_Member_MemberRegistrationNumber",
                        column: x => x.MemberRegistrationNumber,
                        principalTable: "Member",
                        principalColumn: "MemberRegistrationNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthRegistration_MemberRegistrationNumber",
                table: "HealthRegistration",
                column: "MemberRegistrationNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthRegistration");

            migrationBuilder.DropTable(
                name: "Member");
        }
    }
}
