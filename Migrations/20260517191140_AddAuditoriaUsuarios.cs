using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuebleriaProfe.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoriaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "Ventas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "Ventas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "VentaDetalles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "VentaDetalles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "Productos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "Productos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "PlanesPago",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "PlanesPago",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "Gastos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "Gastos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "Clientes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "Abonos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioUuid",
                table: "Abonos",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "VentaDetalles");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "PlanesPago");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "Abonos");

            migrationBuilder.DropColumn(
                name: "UsuarioUuid",
                table: "Abonos");
        }
    }
}
