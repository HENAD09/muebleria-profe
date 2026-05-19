using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuebleriaProfe.Migrations
{
    /// <inheritdoc />
    public partial class ActualizandoVentasOffline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoVenta",
                table: "Ventas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "ClienteNombre",
                table: "Ventas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "VentaDetalles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductoNombre",
                table: "VentaDetalles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClienteNombre",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "ProductoNombre",
                table: "VentaDetalles");

            migrationBuilder.AlterColumn<string>(
                name: "TipoVenta",
                table: "Ventas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
