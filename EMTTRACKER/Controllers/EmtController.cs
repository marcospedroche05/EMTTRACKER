using EMTTRACKER.Extensions;
using EMTTRACKER.Filters;
using EMTTRACKER.Models;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMTTRACKER.Controllers
{
    public class EmtController : Controller
    {
        IRepositoryEmt repo;

        public EmtController(IRepositoryEmt repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<VParadaUrbana> paradas = await this.repo.GetAllParadasUrbano();
            if (paradas != null)
            {
                paradas = paradas.OrderBy(x => x.Codigo).ToList();
            }
            return View(paradas);
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(int codigo)
        {
            VParadaUrbana parada = await this.repo.FindParadaUrbanoByCodigoAsync(codigo);
            if(parada != null)
            {
                ViewData["PARADABUSCADA"] = parada;
            }
            return View();
        }


        public async Task<IActionResult> Horas(int codigo)
        {
            var paradaReal = await this.repo.GetParadaByCodigo(codigo);
            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                Favorita paradaFavorita = await this.repo.FindFavoritaAsync(usuario, paradaReal.IdParada);

                if (paradaFavorita != null)
                {
                    ViewData["NOMBREPARADA"] = paradaFavorita.Alias;
                    ViewData["ESFAVORITA"] = true;
                } else
                {
                    ViewData["NOMBREPARADA"] = paradaReal.Nombre;
                    ViewData["ESFAVORITA"] = false;
                }
            } else
            {
                ViewData["NOMBREPARADA"] = paradaReal.Nombre;
            }
            List<VHorariosParadaUrbanos> horarios = await this.repo.GetHorariosParadaUrbano(codigo);
            ViewData["CODIGO"] = codigo;
            return View(horarios);
        }

        //VISTA DE HORARIOS.
        public async Task<IActionResult> AgregarFavorita(int codigo)
        {
            int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            VParadaUrbana parada = await this.repo.FindParadaUrbanoByCodigoAsync(codigo);
            var paradaReal = await this.repo.GetParadaByCodigo(codigo);
            // Agregar a favoritos y redirigir con mensaje de éxito
            ViewData["CODIGO"] = codigo;
            await this.repo.InsertFavoritaAsync(usuario, paradaReal.IdParada, parada.Nombre);
            return RedirectToAction("Horas", new { codigo = codigo });
        }

        //VISTA DE HORARIOS.
        public async Task<IActionResult> EliminarFavorita(int codigo)
        {
            int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var paradaReal = await this.repo.GetParadaByCodigo(codigo);
            await this.repo.DeleteFavoritaAsync(usuario, paradaReal.IdParada);
            return RedirectToAction("Horas", new { codigo = codigo });
        }

        //VISTA DE HORARIOS. PERMITE CAMBIAR EL NOMBRE DE TU PARADA FAVORITA
        [HttpPost]
        public async Task<IActionResult> AsignarAlias(int codigo, string nuevoAlias)
        {
            if(HttpContext.User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Login");
            }
            int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var paradaReal = await this.repo.GetParadaByCodigo(codigo);
            await this.repo.AsignarAlias(usuario, paradaReal.IdParada, nuevoAlias);
            TempData["MENSAJE"] = "Alias modificado correctamente";
            return RedirectToAction("Horas", new { codigo = codigo });
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
                List<VParadaUrbana> favoritasUrbano = await this.repo.GetFavoritasUrbanasAsync(usuario);
                if (favoritasUrbano != null)
                {
                    favoritasUrbano = favoritasUrbano.OrderBy(x => x.Codigo).ToList();
                }
                ViewData["FAVORITAS"] = favoritasUrbano;
                return View("Index");
            }
        }
    }
}
