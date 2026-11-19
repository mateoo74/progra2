using System.ComponentModel.DataAnnotations;

namespace StreamHubApi.Models
{
    public class Contenido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Tipo { get; set; } = string.Empty; // "Película" o "Serie"

        [Required]
        [MaxLength(50)]
        public string Genero { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string ClasificacionEdad { get; set; } = string.Empty; // Ej: "ATP", "+13", "+16", "+18"

        [Required]
        public string UrlStream { get; set; } = string.Empty;
    }
}