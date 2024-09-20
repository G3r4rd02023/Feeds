using Feeds.Backend.Data;
using Feeds.Shared.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly DataContext _context;

        public ComentariosController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Comentarios.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Comentario comentario)
        {
            var usuario = _context.Usuarios.FirstOrDefault(e => e.Id == comentario.UsuarioId);
            comentario.Usuario = usuario;
            _context.Add(comentario);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var comentario = await _context.Comentarios
                .SingleOrDefaultAsync(p => p.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }
            return Ok(comentario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Comentario comentario)
        {
            if (id != comentario.Id)
            {
                return BadRequest();
            }

            _context.Update(comentario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }
            _context.Remove(comentario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}