using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using prjMvcCoreDemo.Models;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class NL_ChatHubController : Controller
    {  
        public IActionResult Index()
        {
            return View();
        }
    }
}
