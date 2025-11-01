namespace Firmness.Application.Interfaces;
    public interface IIdentityService
    {
        /// <summary>
        /// Crea un usuario en el store de identidad y devuelve su Id (string) si la operación tuvo éxito; null en caso contrario.
        /// </summary>
        Task<string?> CreateUserAsync(string email, string password, string? firstName = null, string? lastName = null);

        /// <summary>
        /// Comprueba la contraseña del usuario identificado por userId.
        /// </summary>
        Task<bool> CheckPasswordAsync(string userId, string password);

        /// <summary>
        /// Devuelve los roles asociados al usuario.
        /// </summary>
        Task<IReadOnlyList<string>> GetRolesAsync(string userId);

        /// <summary>
        /// Añade un usuario a un rol (crea el rol si no existe).
        /// </summary>
        Task AddToRoleAsync(string userId, string role);

        /// <summary>
        /// Busca un usuario por email y devuelve información básica.
        /// </summary>
        Task<ApplicationUserDto?> GetByEmailAsync(string email);
    }
    public record ApplicationUserDto(string Id, string Email, string? UserName);