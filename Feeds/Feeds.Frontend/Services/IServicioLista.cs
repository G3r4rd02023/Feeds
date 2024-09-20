using Feeds.Shared.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Feeds.Frontend.Services
{
    public interface IServicioLista
    {
        Task<IEnumerable<SelectListItem>> GetListaCategorias(Usuario usuario);
    }
}