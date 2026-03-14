using EMTTRACKER.Extensions;
using EMTTRACKER.Models;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMTTRACKER.Controllers
{
    public class InterurbanoController : Controller
    {
        IRepositoryInterurbano repo;

        public InterurbanoController(IRepositoryInterurbano repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<VParadaInterurbana> paradas = await this.repo.GetAllParadasInterurbanas();
            if (paradas != null)
            {
                paradas = paradas.OrderBy(x => x.Codigo).ToList();
            }
            return View(paradas);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int codigo)
        {
            VParadaInterurbana parada = await this.repo.FindParadaInterurbanoByCodigoAsync(codigo);
            if (parada != null)
            {
                ViewData["PARADABUSCADA"] = parada;
            }
            return View();
        }

        public async Task<IActionResult> Horas(int codigo)
        {
            var paradaReal = await this.repo.GetParadaByCodigo(codigo);
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
            List<VHorariosParadaInterurbanos> horarios = await this.repo.GetHorariosParadaInterurbano(codigo);
            ViewData["CODIGO"] = codigo;
            return View(horarios);
        }

        //VISTA DE HORARIOS.
        public async Task<IActionResult> AgregarFavorita(int codigo)
        {
            int usuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            VParadaInterurbana parada = await this.repo.FindParadaInterurbanoByCodigoAsync(codigo);
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
            if (HttpContext.User.Identity.IsAuthenticated == false)
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
                List<VParadaInterurbana> favoritasInter = await this.repo.GetFavoritasInterurbanasAsync(usuario);
                if (favoritasInter != null)
                {
                    favoritasInter = favoritasInter.OrderBy(x => x.Codigo).ToList();
                }
                ViewData["FAVORITAS"] = favoritasInter;
                return View("Index");
            }
        }

    }
}
