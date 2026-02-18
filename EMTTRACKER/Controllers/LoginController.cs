using EMTTRACKER.Models;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMTTRACKER.Controllers
{
    public class LoginController : Controller
    {
        IRepositoryLogin repo;

        public LoginController(IRepositoryLogin repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            ViewData["MENSAJE"] = "";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UsuarioLogado usuarioLogado)
        {
            string token = await this.repo.Login(usuarioLogado.Email, usuarioLogado.Password);
            if(token == null)
            {
                ViewData["MENSAJE"] = "Correo electronico o contraseña incorrectos";
                return View();
            } else
            {
                Usuario user = await this.repo.FindUsuarioEmailAsync(usuarioLogado.Email);
                // 2. Creamos la identidad (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("IdUsuario", user.IdUsuario.ToString()),
                    // Puedes añadir roles si los tienes: new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // 3. ¡Aquí ocurre la magia! El servidor crea la Cookie y la envía al navegador
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
