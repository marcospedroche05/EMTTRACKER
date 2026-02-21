using Microsoft.AspNetCore.Mvc;

namespace EMTTRACKER.Controllers
{
    public class EmtController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
