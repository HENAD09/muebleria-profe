using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuebleriaProfe.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposFrecuenciaPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SemanasOfertaContado",
                table: "PlanesPago",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoPeriodo",
                table: "PlanesPago",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalPeriodos",
                table: "PlanesPago",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SemanasOfertaContado",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "TipoPeriodo",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "TotalPeriodos",
                table: "PlanesPago");
        }
    }
}
