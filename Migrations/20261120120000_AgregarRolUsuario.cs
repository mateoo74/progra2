using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamHubApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRolUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Los usuarios que ya existen quedan con el rol "Usuario".
            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Usuarios");
        }
    }
}
