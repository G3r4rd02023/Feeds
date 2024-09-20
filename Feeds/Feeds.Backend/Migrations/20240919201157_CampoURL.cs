using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feeds.Backend.Migrations
{
    /// <inheritdoc />
    public partial class CampoURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Categorias_CategoriaId",
                table: "Entradas");

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Entradas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Categorias_CategoriaId",
                table: "Entradas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Categorias_CategoriaId",
                table: "Entradas");

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Entradas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Categorias_CategoriaId",
                table: "Entradas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }
    }
}
