using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FellerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPropiedadesConcesionaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Auto_Kilometraje",
                table: "Vehiculos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cilindrada",
                table: "Vehiculos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Vehiculos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Kilometraje",
                table: "Vehiculos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Puertas",
                table: "Vehiculos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCombustible",
                table: "Vehiculos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMoto",
                table: "Vehiculos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Transmision",
                table: "Vehiculos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auto_Kilometraje",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Cilindrada",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Kilometraje",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Puertas",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "TipoCombustible",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "TipoMoto",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Transmision",
                table: "Vehiculos");
        }
    }
}
