using EMTTRACKER.Extensions;
using EMTTRACKER.Models;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Mvc;

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
            List<VHorariosParadaUrbanos> horarios = await this.repo.GetHorariosParadaUrbano(codigo);
            ViewData["CODIGO"] = codigo;
            return View(horarios);
        }

        public async Task<IActionResult> AgregarFavorita(int codigo)
        {
            Usuario usuario = HttpContext.Session.GetObject<Usuario>("USUARIO");
            VParadaUrbana parada = await this.repo.FindParadaUrbanoByCodigoAsync(codigo);
            var paradaReal = await this.repo.GetParadaByCodigo(codigo);
            await this.repo.InsertFavoritaAsync(usuario.IdUsuario, paradaReal.IdParada, parada.Nombre);
            return RedirectToAction("Index");
        }


        //public async Task<IActionResult> GetFavoritasEmt()
        //{
        //    if(HttpContext.Session.GetObject<Usuario>("USUARIO") == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    } else
        //    {

        //        return View();
        //    }
        //}
    }
}
