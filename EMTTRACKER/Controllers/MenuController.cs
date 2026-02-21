using Microsoft.AspNetCore.Mvc;

namespace EMTTRACKER.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
