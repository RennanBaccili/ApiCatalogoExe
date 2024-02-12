using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiCatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES('Bebidas', 'bebidas.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES('Comida', 'Lanches.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES('Doce', 'Doces.jpg')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Categorias");
        }
    }
}
