using Feeds.Backend.Data;
using Feeds.Shared.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Feeds.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly DataContext _context;

        public ImagesController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Imagen imagen)
        {
            _context.Add(imagen);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}