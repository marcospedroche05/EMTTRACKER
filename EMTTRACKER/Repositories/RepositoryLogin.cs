using EMTTRACKER.Data;
using EMTTRACKER.Helpers;
using EMTTRACKER.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

#region VIEWS

//CREATE VIEW V_USUARIO_SEGURIDAD
//AS
//	SELECT U.IDUSUARIO, U.NOMBRE, U.EMAIL, US.SALT, US.PASS FROM USUARIOS U
//	INNER JOIN USERS_SECURITY US ON U.IDUSUARIO = US.IDUSUARIO
//GO

#endregion

namespace EMTTRACKER.Repositories
{
    public class RepositoryLogin: IRepositoryLogin
    {
        EmtContext context;

        public RepositoryLogin(EmtContext context)
        {
            this.context = context; 
        }

        public async Task<VUsuarioSeguridad> FindUsuarioSeguridadEmailAsync(string email)
        {
            var consulta = from datos in this.context.VistaUsuariosSeguridad
                           where datos.Email == email
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<Usuario> FindUsuarioEmailAsync(string email)
        {
            var consulta = from datos in this.context.Usuarios
                           where datos.Email == email
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<VUsuarioSeguridad> LogInUserAsync(string email, string password)
        {
            VUsuarioSeguridad usuario = await FindUsuarioSeguridadEmailAsync(email);
            if(usuario == null)
            {
                return null;
            } else {
                string salt = usuario.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passBytes = usuario.Pass;
                bool response = HelperTools.CompareArrays(temp, passBytes);
                if(response == true)
                {
                    return usuario;
                } else
                {
                    return null;
                }
            }
        }

        public async Task RegistrarAsync(string nombre, string email, string password)
        {
            Usuario nuevoUsuario = await InsertNuevoUsuario(nombre, email, password);
            UsuarioSeguridad usuarioSeguridad = new UsuarioSeguridad();
            usuarioSeguridad.IdUsuario = nuevoUsuario.IdUsuario;
            usuarioSeguridad.Salt = HelperTools.GenerateSalt();
            usuarioSeguridad.Pass = HelperCryptography.EncryptPassword(password, usuarioSeguridad.Salt);
            await this.context.UsuariosSeguridad.AddAsync(usuarioSeguridad);
            await this.context.SaveChangesAsync();
        }

        private async Task<Usuario> InsertNuevoUsuario(string nombre, string email, string password)
        {
            int idUsuario = await GetUltimoIdUsuario() + 1;
            Usuario nuevoUsuario = new Usuario
            {
                IdUsuario = idUsuario,
                Nombre = nombre,
                Email = email,
                Password = password,
                Rol = "USUARIO"
            };
            await this.context.Usuarios.AddAsync(nuevoUsuario);
            await this.context.SaveChangesAsync();
            return nuevoUsuario;
        }
        
        private async Task<int> GetUltimoIdUsuario()
        {
            var consulta = from datos in this.context.Usuarios
                           select datos;
            List<Usuario> usuarios = await consulta.ToListAsync();
            Usuario user = usuarios.MaxBy(x => x.IdUsuario);
            if (user == null)
            {
                return 0;
            }
            else
            {
                return user.IdUsuario;
            }
        }
    }
}
