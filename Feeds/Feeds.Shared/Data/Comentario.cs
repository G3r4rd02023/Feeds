using System.ComponentModel.DataAnnotations;

namespace Feeds.Shared.Data
{
    public class Comentario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Contenido { get; set; } = null!;

        public DateTime FechaComentario { get; set; } = DateTime.Now;

        public Usuario? Usuario { get; set; }

        public Entrada? Entrada { get; set; }
    }
}