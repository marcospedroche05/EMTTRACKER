using EMTTRACKER.Data;
using EMTTRACKER.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EMTTRACKER.Repositories
{
    public class RepositoryLogin: IRepositoryLogin
    {
        EmtContext context;

        public RepositoryLogin(EmtContext context)
        {
            this.context = context; 
        }

        public async Task<Usuario> FindUsuarioEmailAsync(string email)
        {
            var consulta = from datos in this.context.Usuarios
                           where datos.Email == email
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<bool> CheckPassword(string email, string password)
        {
            Usuario user = await FindUsuarioEmailAsync(email);
            if (password == user.Password)
            {
                return true;
            }
            else return false;
        }
    }
}
