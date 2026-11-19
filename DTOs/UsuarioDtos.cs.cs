using System.ComponentModel.DataAnnotations;

namespace StreamHubApi.DTOs
{
    // Entrada para registrar un usuario nuevo
    public class UsuarioRegisterDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }

    // Entrada para iniciar sesión y obtener el JWT
    public class UsuarioLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Salida limpia del usuario (sin PasswordHash)
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }

    // Salida del login con el token JWT
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioResponseDto Usuario { get; set; } = new();
    }
}