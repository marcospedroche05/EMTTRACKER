using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryLogin
    {
        public Task<Usuario> FindUsuarioEmailAsync(string email);
        public Task<bool> CheckPassword(string email, string password);
    }
}
