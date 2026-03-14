using EMTTRACKER.Filters;
using EMTTRACKER.Models;
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

        [AuthorizeAdmin(Policy = "ADMINONLY")]
        public async Task<IActionResult> Index()
        {
            var lista = await this.repo.GetAllIncidenciasAsync();
            return View(lista);
        }

        public async Task<IActionResult> GetIncidenciasJson()
        {
            var lista = await this.repo.GetIncidenciasAsync();
            return Ok(lista);
        }

        [AuthorizeAdmin(Policy = "ADMINONLY")]
        public IActionResult Create()
        {
            return View();
        }

        [AuthorizeAdmin(Policy = "ADMINONLY")]
        [HttpPost]
        public async Task<IActionResult> Create(string titulo, string mensaje, DateTime fechainicio, DateTime fechafin)
        {
            await this.repo.InsertIncidenciaAsync(titulo, mensaje, fechainicio, fechafin);
            return RedirectToAction("Index");
        }

        [AuthorizeAdmin(Policy = "ADMINONLY")]
        public async Task<IActionResult> Edit(int id)
        {
            Incidencia incidencia = await this.repo.FindIncidenciaAsync(id);
            if(incidencia == null) return RedirectToAction("Index");
            return View(incidencia);
        }

        [AuthorizeAdmin(Policy = "ADMINONLY")]
        [HttpPost]
        public async Task<IActionResult> Edit(int idincidencia, string titulo, string mensaje, DateTime fechainicio, DateTime fechafin)
        {
            await this.repo.UpdateIncidenciaAsync(idincidencia, titulo, mensaje, fechainicio, fechafin);
            return RedirectToAction("Index");
        }

        [AuthorizeAdmin(Policy = "ADMINONLY")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.repo.DeleteIncidenciaAsync(id);
            return RedirectToAction("Index");
        }
    }
}
