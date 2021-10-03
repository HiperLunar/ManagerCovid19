using Microsoft.EntityFrameworkCore.Migrations;

namespace ManagerCovid19.Migrations
{
    public partial class Add_Symp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symptoms",
                table: "HealthRegistration");

            migrationBuilder.AddColumn<bool>(
                name: "Calafrios",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Cansaco",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Coriza",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Diarreia",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DorDeCabeca",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DorDeGarganta",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DorNoPeito",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Espirros",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FaltaDeAr",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Febre",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerdaDeOlfato",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerdaPaladar",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Tosse",
                table: "HealthRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calafrios",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "Cansaco",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "Coriza",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "Diarreia",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "DorDeCabeca",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "DorDeGarganta",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "DorNoPeito",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "Espirros",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "FaltaDeAr",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "Febre",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "PerdaDeOlfato",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "PerdaPaladar",
                table: "HealthRegistration");

            migrationBuilder.DropColumn(
                name: "Tosse",
                table: "HealthRegistration");

            migrationBuilder.AddColumn<string>(
                name: "Symptoms",
                table: "HealthRegistration",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
