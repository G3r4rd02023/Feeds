using Feeds.Shared.Data;
using Feeds.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await ValidarUsuariosAsync("SuperAdmin", "superadmin@gmail.com", "123456", Rol.Administrador);
        }

        private async Task<Usuario> ValidarUsuariosAsync(string nombre, string correo, string password, Rol administrador)
        {
            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == correo);
            if (usuarioExistente != null)
            {
                return usuarioExistente;
            }

            Usuario usuario = new()
            {
                Contrasena = password,
                CorreoElectronico = correo,
                FechaRegistro = DateTime.Now,
                Nombre = nombre,
                Rol = Rol.Administrador,
                URLFoto = "https://res.cloudinary.com/dbsaxzz05/image/upload/v1725662135/dqsw7mavp77po9xwjjgw.jpg"
            };

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}