using Feeds.Shared.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feeds.Frontend.Models
{
    public class PublicacionViewModel : Entrada
    {
        [NotMapped]
        public IEnumerable<SelectListItem>? Categorias { get; set; }
    }
}