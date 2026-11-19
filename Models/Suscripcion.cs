using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamHubApi.Models
{
    public class Suscripcion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoPlan { get; set; } = string.Empty; // Ej: "Básico", "Estándar", "Premium"

        [Required]
        public DateTime FechaInicioPeriodo { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "Pendiente de Pago"; // "Activa", "Cancelada", "Pendiente de Pago"
    }
}