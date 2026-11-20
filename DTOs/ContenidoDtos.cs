using System.ComponentModel.DataAnnotations;

namespace StreamHubApi.DTOs
{
    // Entrada para crear o actualizar contenido (POST / PUT)
    public class ContenidoCreateUpdateDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio (Película o Serie).")]
        [MaxLength(20)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es obligatorio.")]
        [MaxLength(50)]
        public string Genero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La clasificación de edad es obligatoria.")]
        [MaxLength(10)]
        public string ClasificacionEdad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La URL del stream es obligatoria.")]
        public string UrlStream { get; set; } = string.Empty;
    }

    // Salida pública para el catálogo y detalle (oculta UrlStream por seguridad)
    public class ContenidoCatalogoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string ClasificacionEdad { get; set; } = string.Empty;
    }

    // Salida solo para el Administrador: incluye UrlStream para poder editar el contenido
    public class ContenidoAdminDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string ClasificacionEdad { get; set; } = string.Empty;
        public string UrlStream { get; set; } = string.Empty;
    }

    // Salida exclusiva del endpoint GET /content/{content_id}/play cuando la suscripción está Activa
    public class PlayContentResponseDto
    {
        public int ContentId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string UrlStream { get; set; } = string.Empty;
        public string Mensaje { get; set; } = "Acceso concedido.";
    }
}