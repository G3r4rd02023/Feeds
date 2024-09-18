using Feeds.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Feeds.Shared.Data
{
    public class Usuario
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(256, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string CorreoElectronico { get; set; } = null!;

        [MaxLength(256, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Contrasena { get; set; } = null!;

        public Rol Rol { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string? URLFoto { get; set; }
    }
}