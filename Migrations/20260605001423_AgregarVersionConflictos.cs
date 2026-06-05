using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuebleriaProfe.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVersionConflictos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Ventas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "VentaDetalles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Productos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PlanesPago",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Gastos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Clientes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Abonos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Abonos");
        }
    }
}
