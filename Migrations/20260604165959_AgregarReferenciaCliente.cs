using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuebleriaProfe.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReferenciaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Referencia",
                table: "Clientes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "Clientes");
        }
    }
}
