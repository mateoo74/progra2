using System.ComponentModel.DataAnnotations;

namespace StreamHubApi.Models
{
    public class PasarelaPago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string UrlBase { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;
    }
}