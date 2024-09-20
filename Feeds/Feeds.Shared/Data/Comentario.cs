using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Feeds.Shared.Data
{
    public class Comentario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Contenido { get; set; } = null!;

        public DateTime FechaComentario { get; set; } = DateTime.Now;

        public Usuario? Usuario { get; set; }
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public Entrada? Entrada { get; set; }

        public int EntradaId { get; set; }
    }
}