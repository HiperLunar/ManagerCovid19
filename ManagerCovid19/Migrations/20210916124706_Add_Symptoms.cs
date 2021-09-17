using Microsoft.EntityFrameworkCore.Migrations;

namespace ManagerCovid19.Migrations
{
    public partial class Add_Symptoms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Symptoms",
                table: "HealthRegistration",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symptoms",
                table: "HealthRegistration");
        }
    }
}
