using StreamHubApi.DTOs;

namespace StreamHubApi.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDto?> RegistrarAsync(UsuarioRegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(UsuarioLoginDto dto);
        Task<UsuarioResponseDto?> ObtenerPorIdAsync(int id);
    }
}