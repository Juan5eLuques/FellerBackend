using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FellerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVehiculosDestacados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Transmision",
                table: "Vehiculos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoMoto",
                table: "Vehiculos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoCombustible",
                table: "Vehiculos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Nafta",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Puertas",
                table: "Vehiculos",
                type: "integer",
                nullable: true,
                defaultValue: 4,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Vehiculos",
                type: "text",
                nullable: false,
                defaultValue: "Usado",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Cilindrada",
                table: "Vehiculos",
                type: "integer",
                nullable: true,
                defaultValue: 150,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsDestacado",
                table: "Vehiculos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrdenDestacado",
                table: "Vehiculos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_EsDestacado_OrdenDestacado",
                table: "Vehiculos",
                columns: new[] { "EsDestacado", "OrdenDestacado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_EsDestacado_OrdenDestacado",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "EsDestacado",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "OrdenDestacado",
                table: "Vehiculos");

            migrationBuilder.AlterColumn<string>(
                name: "Transmision",
                table: "Vehiculos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoMoto",
                table: "Vehiculos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoCombustible",
                table: "Vehiculos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "Nafta");

            migrationBuilder.AlterColumn<int>(
                name: "Puertas",
                table: "Vehiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Vehiculos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Usado");

            migrationBuilder.AlterColumn<int>(
                name: "Cilindrada",
                table: "Vehiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 150);
        }
    }
}
