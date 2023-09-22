using Microsoft.AspNetCore.Mvc;
using prjEnGeeDemo.Models;

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
