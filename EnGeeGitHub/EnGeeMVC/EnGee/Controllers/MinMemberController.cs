using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit;
using EnGee.ViewModels;

namespace EnGee.Controllers
{
    public class MinMemberController : Controller
    {
        public IActionResult EmailValid()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TMember tm)
        {
            EngeeContext db = new EngeeContext();

            db.Add(tm);
            db.SaveChanges();
            return RedirectToAction("EmailValid");
        }

    }
}

