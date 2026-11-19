using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamHubApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarBoletas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boletas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SuscripcionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoPago = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Periodo = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boletas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boletas_Suscripciones_SuscripcionId",
                        column: x => x.SuscripcionId,
                        principalTable: "Suscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boletas_SuscripcionId",
                table: "Boletas",
                column: "SuscripcionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boletas");
        }
    }
}
