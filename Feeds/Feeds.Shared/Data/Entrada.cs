using Feeds.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Feeds.Shared.Data
{
    public class Entrada
    {
        public int Id { get; set; }

        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Titulo { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Contenido { get; set; } = null!;

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        public Estado Estado { get; set; }

        public string? Etiquetas { get; set; }

        public Usuario? Usuario { get; set; }

        public int UsuarioId { get; set; }

        public Categoria? Categoria { get; set; }

        public int CategoriaId { get; set; }

        public string? URLImagen { get; set; }

        public int Likes { get; set; }

        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}