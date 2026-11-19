using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamHubApi.Context;
using StreamHubApi.DTOs;
using StreamHubApi.Interfaces;
using StreamHubApi.Models;

namespace StreamHubApi.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly StreamHubDbContext _context;

        public UsuarioService(StreamHubDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioResponseDto?> RegistrarAsync(UsuarioRegisterDto dto)
        {
            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (existeEmail) return null;

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = HashearPassword(dto.Password),
                FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return new UsuarioResponseDto
            {
                Id = nuevoUsuario.Id,
                Nombre = nuevoUsuario.Nombre,
                Email = nuevoUsuario.Email,
                FechaRegistro = nuevoUsuario.FechaRegistro
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(UsuarioLoginDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null) return null;

            var hashIngresado = HashearPassword(dto.Password);
            if (usuario.PasswordHash != hashIngresado) return null;

            var token = GenerarJwtToken(usuario);

            return new AuthResponseDto
            {
                Token = token,
                Usuario = new UsuarioResponseDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    FechaRegistro = usuario.FechaRegistro
                }
            };
        }

        public async Task<UsuarioResponseDto?> ObtenerPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return null;

            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                FechaRegistro = usuario.FechaRegistro
            };
        }

        private static string HashearPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        private static string GenerarJwtToken(Usuario usuario)
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? throw new InvalidOperationException("Falta JWT_SECRET_KEY en .env");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "StreamHubApi";
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "StreamHubClient";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}