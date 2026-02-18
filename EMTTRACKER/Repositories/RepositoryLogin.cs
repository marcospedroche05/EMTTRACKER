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

        public async Task<string> Login(string email, string password)
        {
            Usuario user = await FindUsuarioEmailAsync(email);
            if (user == null || await CheckPassword(email, password) == false)
            {
                return null;
            }

            var claims = new[]
            {           
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Tu_Clave_Secreta_Super_Larga_Y_Segura"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(8), // Duración
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
