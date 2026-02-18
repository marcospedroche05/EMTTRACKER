using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryLogin
    {
        public Task<Usuario> FindUsuarioEmailAsync(string email);
        public Task<bool> CheckPassword(string email, string password);
        public Task<string> Login(string email, string password);
    }
}
