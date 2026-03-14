using EMTTRACKER.Extensions;
using EMTTRACKER.Models;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EMTTRACKER.Controllers
{
    public class CercaniasController : Controller
    {
        IRepositoryCercanias repo;

        public CercaniasController(IRepositoryCercanias repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<VParadaCercanias> paradas = await this.repo.GetAllParadasCercanias();
            if (paradas != null)
            {
                paradas = paradas.OrderBy(x => x.Nombre).ToList();
            }
            return View(paradas);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string nombre)
        {
            VParadaCercanias parada = await this.repo.FindParadaCercaniasByNombreAsync(nombre);
            if (parada != null)
            {
                ViewData["PARADABUSCADA"] = parada;
            }
            return View();
        }

        public async Task<IActionResult> Horas(int idparada)
        {
            var paradaReal = await this.repo.GetParadaById(idparada);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                Favorita paradaFavorita = await this.repo.FindFavoritaAsync(usuario, paradaReal.IdParada);

                if (paradaFavorita != null)
                {
                    ViewData["NOMBREPARADA"] = paradaFavorita.Alias;
                    ViewData["ESFAVORITA"] = true;
                }
                else
                {
                    ViewData["NOMBREPARADA"] = paradaReal.Nombre;
                    ViewData["ESFAVORITA"] = false;
                }
            }
            else
            {
                ViewData["NOMBREPARADA"] = paradaReal.Nombre;
            }
            List<VHorariosParadaCercanias> horarios = await this.repo.GetHorariosParadaCercanias(paradaReal.IdParada);
            ViewData["IDPARADA"] = idparada;
            return View(horarios);
        }

        //VISTA DE HORARIOS.
        public async Task<IActionResult> AgregarFavorita(int idparada)
        {
            int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var paradaReal = await this.repo.GetParadaById(idparada);
            VParadaCercanias parada = await this.repo.FindParadaCercaniasByNombreAsync(paradaReal.Nombre);
            // Agregar a favoritos y redirigir con mensaje de éxito
            ViewData["IDPARADA"] = idparada;
            await this.repo.InsertFavoritaAsync(usuario, paradaReal.IdParada, parada.Nombre);
            return RedirectToAction("Horas", new { idparada = idparada });
        }

        //VISTA DE HORARIOS.
        public async Task<IActionResult> EliminarFavorita(int idparada)
        {
            int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var paradaReal = await this.repo.GetParadaById(idparada);
            await this.repo.DeleteFavoritaAsync(usuario, paradaReal.IdParada);
            return RedirectToAction("Horas", new { idparada = idparada });
        }

        //VISTA DE HORARIOS. PERMITE CAMBIAR EL NOMBRE DE TU PARADA FAVORITA
        [HttpPost]
        public async Task<IActionResult> AsignarAlias(int idparada, string nuevoAlias)
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Login");
            }
            int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var paradaReal = await this.repo.GetParadaById(idparada);
            await this.repo.AsignarAlias(usuario, paradaReal.IdParada, nuevoAlias);
            TempData["MENSAJE"] = "Alias modificado correctamente";
            return RedirectToAction("Horas", new { idparada = idparada });
        }

        //VISTA DE INDEX/BUSCADOR. ACTUALIZA LA LISTA CON UNICAMENTE PARADAS FAVORITAS
        public async Task<IActionResult> GetFavoritas()
        {
            if (HttpContext.User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                List<VParadaCercanias> favoritasCercanias = await this.repo.GetFavoritasCercaniasAsync(usuario);
                if (favoritasCercanias != null)
                {
                    favoritasCercanias = favoritasCercanias.OrderBy(x => x.Nombre).ToList();
                }
                ViewData["FAVORITAS"] = favoritasCercanias;
                return View("Index");
            }
        }
    }
}
