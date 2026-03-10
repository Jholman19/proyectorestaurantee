using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixHuellaTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "HuellaTemplate",
                table: "Clientes",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HuellaTemplate",
                table: "Clientes");
        }
    }
}
