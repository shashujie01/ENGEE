using Microsoft.AspNetCore.Mvc;

namespace EnGee.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
