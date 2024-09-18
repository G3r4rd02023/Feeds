using System.ComponentModel.DataAnnotations;

namespace Feeds.Shared.Models
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(256)]
        public string CorreoElectronico { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string Contrasena { get; set; } = null!;
    }
}