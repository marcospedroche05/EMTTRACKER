using EMTTRACKER.Extensions;
using EMTTRACKER.Helpers;
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
        public async Task<IActionResult> Index(string email, string password)
        {
            VUsuarioSeguridad usuarioLogado = await this.repo.LogInUserAsync(email, password);
            if(usuarioLogado == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            } else
            {
                Usuario user = await this.repo.FindUsuarioEmailAsync(email);
                HttpContext.Session.SetObject("USUARIO", user);
                return RedirectToAction("Index", "Menu");
            }    
        }

        public IActionResult Logout()
        {
            if(HttpContext.Session.GetObject<Usuario>("USUARIO") != null)
            {
                HttpContext.Session.SetObject("USUARIO", null);
            }
            return RedirectToAction("Index", "Menu");
        }

        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string password)
        {
            if(await this.repo.FindUsuarioEmailAsync(email) != null)
            {
                ViewData["MENSAJE"] = "Ya existe un usuario con este email";
                return View();
            } 
            else
            {
                await this.repo.RegistrarAsync(nombre, email, password);
                return RedirectToAction("Index");
            }
        }
    }
}
