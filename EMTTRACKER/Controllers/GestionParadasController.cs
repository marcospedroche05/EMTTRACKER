using EMTTRACKER.Models;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EMTTRACKER.Controllers
{
    public class GestionParadasController : Controller
    {
        private IRepositoryGestionParadas repo;

        public GestionParadasController(IRepositoryGestionParadas repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var paradas = await this.repo.GetAllParadasAsync();
            return View(paradas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int? codigo, string nombre)
        {
            await this.repo.InsertParadaAsync(codigo, nombre);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            Parada parada = await this.repo.FindParadaAsync(id);
            if (parada == null)
            {
                return RedirectToAction("Index");
            }
            return View(parada);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int idparada, int? codigo, string nombre)
        {
            await this.repo.UpdateParadaAsync(idparada, codigo, nombre);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await this.repo.DeleteParadaAsync(id);
            return RedirectToAction("Index");
        }
    }
}
