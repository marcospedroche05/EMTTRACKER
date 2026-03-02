using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EMTTRACKER.Controllers
{
    public class IncidenciasController : Controller
    {
        IRepositoryIncidencias repo;
        public IncidenciasController(IRepositoryIncidencias repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetIncidenciasJson()
        {
            var lista = await this.repo.GetIncidenciasAsync();
            return Ok(lista);
        }
    }
}
