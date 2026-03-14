using Microsoft.AspNetCore.Mvc;

namespace EMTTRACKER.Controllers
{
    public class ManagedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
