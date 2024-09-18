using Feeds.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<Entrada> Entradas { get; set; }

        public DbSet<Imagen> Imagenes { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}