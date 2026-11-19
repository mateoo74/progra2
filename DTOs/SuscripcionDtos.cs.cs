using System.ComponentModel.DataAnnotations;

namespace StreamHubApi.DTOs
{
    // Entrada para asignar una suscripción a un usuario
    public class SuscripcionCreateDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de plan es obligatorio (Básico, Estándar o Premium).")]
        [MaxLength(50)]
        public string TipoPlan { get; set; } = string.Empty;

        // Opcional al crear: por defecto arranca en "Pendiente de Pago" o podés mandar "Activa" para probar
        [MaxLength(30)]
        public string? Estado { get; set; }
    }

    // Entrada para cambiar el estado ("Activa", "Cancelada", "Pendiente de Pago")
    public class SuscripcionEstadoDto
    {
        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = string.Empty;
    }

    // Salida con los datos de la suscripción
    public class SuscripcionResponseDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string TipoPlan { get; set; } = string.Empty;
        public DateTime FechaInicioPeriodo { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}