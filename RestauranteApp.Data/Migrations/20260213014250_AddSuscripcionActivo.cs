using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSuscripcionActivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suscripciones_Combos_ComboId",
                table: "Suscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Consumos_SuscripcionId_Dia_Tipo",
                table: "Consumos");

            migrationBuilder.DropColumn(
                name: "Fin",
                table: "Suscripciones");

            migrationBuilder.RenameColumn(
                name: "Activa",
                table: "Suscripciones",
                newName: "CreditosDesayunoRestantes");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Consumos",
                newName: "CreadoEn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Inicio",
                table: "Suscripciones",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Suscripciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "CreditosAlmuerzoRestantes",
                table: "Suscripciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreditosCenaRestantes",
                table: "Suscripciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Dia",
                table: "Consumos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Consumos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Numero",
                table: "Consumos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Origen",
                table: "Consumos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoComida",
                table: "Consumos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Avisos",
                columns: table => new
                {
                    AvisoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SuscripcionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Dia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoComida = table.Column<int>(type: "INTEGER", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MarcadoPor = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avisos", x => x.AvisoId);
                    table.ForeignKey(
                        name: "FK_Avisos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Avisos_Suscripciones_SuscripcionId",
                        column: x => x.SuscripcionId,
                        principalTable: "Suscripciones",
                        principalColumn: "SuscripcionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consumos_ClienteId",
                table: "Consumos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumos_SuscripcionId_Dia_TipoComida_Numero",
                table: "Consumos",
                columns: new[] { "SuscripcionId", "Dia", "TipoComida", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Avisos_ClienteId",
                table: "Avisos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Avisos_SuscripcionId_Dia_TipoComida",
                table: "Avisos",
                columns: new[] { "SuscripcionId", "Dia", "TipoComida" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consumos_Clientes_ClienteId",
                table: "Consumos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Suscripciones_Combos_ComboId",
                table: "Suscripciones",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "ComboId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumos_Clientes_ClienteId",
                table: "Consumos");

            migrationBuilder.DropForeignKey(
                name: "FK_Suscripciones_Combos_ComboId",
                table: "Suscripciones");

            migrationBuilder.DropTable(
                name: "Avisos");

            migrationBuilder.DropIndex(
                name: "IX_Consumos_ClienteId",
                table: "Consumos");

            migrationBuilder.DropIndex(
                name: "IX_Consumos_SuscripcionId_Dia_TipoComida_Numero",
                table: "Consumos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "CreditosAlmuerzoRestantes",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "CreditosCenaRestantes",
                table: "Suscripciones");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Consumos");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Consumos");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "Consumos");

            migrationBuilder.DropColumn(
                name: "TipoComida",
                table: "Consumos");

            migrationBuilder.RenameColumn(
                name: "CreditosDesayunoRestantes",
                table: "Suscripciones",
                newName: "Activa");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "Consumos",
                newName: "Tipo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Inicio",
                table: "Suscripciones",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "Fin",
                table: "Suscripciones",
                type: "DATE",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Dia",
                table: "Consumos",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Consumos_SuscripcionId_Dia_Tipo",
                table: "Consumos",
                columns: new[] { "SuscripcionId", "Dia", "Tipo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Suscripciones_Combos_ComboId",
                table: "Suscripciones",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "ComboId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
