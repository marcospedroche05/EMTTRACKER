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
            if(await this.repo.FindUsuarioEmailAsync(email) != null && await this.repo.CheckPassword(email, password) == true)
            {
                Usuario usuario = await this.repo.FindUsuarioEmailAsync(email);
                HttpContext.Session.SetObject("USUARIO", usuario);
                return RedirectToAction("Index", "Menu");
            }
            else
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
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
    }
}
