using System.ComponentModel.DataAnnotations;

namespace StreamHubApi.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Relación 1 a muchos: Un usuario puede tener varias suscripciones (historial)
        public ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
    }
}