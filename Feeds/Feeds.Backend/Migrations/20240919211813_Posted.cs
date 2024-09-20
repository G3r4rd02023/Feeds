using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feeds.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Posted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "URLImagen",
                table: "Entradas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URLImagen",
                table: "Entradas");
        }
    }
}
