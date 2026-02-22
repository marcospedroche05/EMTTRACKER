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
    }
}
