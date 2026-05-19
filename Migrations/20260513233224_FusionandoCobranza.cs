using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuebleriaProfe.Migrations
{
    /// <inheritdoc />
    public partial class FusionandoCobranza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArticuloResumen",
                table: "PlanesPago",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "PlanesPago",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ClienteNombre",
                table: "PlanesPago",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "PlanesPago",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PagoSemanalAcordado",
                table: "PlanesPago",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProximoCobro",
                table: "PlanesPago",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoPendiente",
                table: "PlanesPago",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoVencido",
                table: "PlanesPago",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCredito",
                table: "PlanesPago",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "Abonos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticuloResumen",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "ClienteNombre",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "PagoSemanalAcordado",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "ProximoCobro",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "SaldoPendiente",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "SaldoVencido",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "TotalCredito",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Abonos");
        }
    }
}
