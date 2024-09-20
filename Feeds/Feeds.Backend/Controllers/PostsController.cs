using Feeds.Backend.Data;
using Feeds.Shared.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly DataContext _context;

        public PostsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Entradas
                .Include(e => e.Usuario)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Entrada entrada)
        {
            var usuario = _context.Usuarios.FirstOrDefault(e => e.Id == entrada.UsuarioId);
            entrada.Usuario = usuario;
            _context.Add(entrada);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var entrada = await _context.Entradas
                .SingleOrDefaultAsync(p => p.Id == id);
            if (entrada == null)
            {
                return NotFound();
            }
            return Ok(entrada);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Entrada entrada)
        {
            if (id != entrada.Id)
            {
                return BadRequest();
            }

            _context.Update(entrada);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var entrada = await _context.Entradas.FindAsync(id);
            if (entrada == null)
            {
                return NotFound();
            }
            _context.Remove(entrada);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}