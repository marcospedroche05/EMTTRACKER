using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryLogin
    {
        public Task<VUsuarioSeguridad> FindUsuarioSeguridadEmailAsync(string email);
        public Task<Usuario> FindUsuarioEmailAsync(string email);
        public Task<VUsuarioSeguridad> LogInUserAsync(string email, string password);
        public Task RegistrarAsync(string nombre, string email, string password);
    }
}
