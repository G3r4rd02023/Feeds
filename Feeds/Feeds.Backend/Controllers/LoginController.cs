using Feeds.Backend.Data;
using Feeds.Shared.Data;
using Feeds.Shared.Enums;
using Feeds.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DataContext _context;

        public LoginController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.CorreoElectronico == login.CorreoElectronico);

            if (usuario != null)
            {
                if (BCrypt.Net.BCrypt.Verify(login.Contrasena, usuario.Contrasena))
                {
                    return Ok(new { Message = "Inicio de sesión exitoso.", isSuccess = true });
                }
            }

            return Unauthorized(new { Message = "Inicio de sesión fallido. Usuario o contraseña incorrectos.", isSuccess = false, token = "" });
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            var nuevoUsuario = new Usuario()
            {
                Contrasena = usuario.Contrasena,
                CorreoElectronico = usuario.CorreoElectronico,
                FechaRegistro = DateTime.Now,
                URLFoto = usuario.URLFoto,
                Nombre = usuario.Nombre,
                Rol = Rol.Autor
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario registrado exitosamente." });
        }
    }
}