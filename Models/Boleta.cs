using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamHubApi.Models
{
    public class Boleta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SuscripcionId { get; set; }

        [ForeignKey("SuscripcionId")]
        public Suscripcion? Suscripcion { get; set; }

        [Required]
        [MaxLength(60)]
        public string CodigoPago { get; set; } = string.Empty; 

        [Required]
        [MaxLength(6)]
        public string Periodo { get; set; } = string.Empty; 

        [Required]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente"; 
    }
}