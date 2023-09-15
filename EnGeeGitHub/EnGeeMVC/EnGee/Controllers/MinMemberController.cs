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
            //---------------條件限制規則--------------------//
            if (tm.RegistrationDate == null)
            {
                tm.RegistrationDate = DateTime.Now;  //註冊日預設當日
            }
            if (tm.Access == null)
            {
                if(tm.CharityProof != null)
                {
                    tm.Access = 3;  //公益團體權限3
                }
                else
                {
                    tm.Access = 1; //一般使用者
                }
            }
            if (tm.Point == null)
            {
                tm.Point = 0;
            }
            //-------------------------------------------//


            EngeeContext db = new EngeeContext();

            db.Add(tm);
            db.SaveChanges();
            return RedirectToAction("EmailValid");
        }

    }
}

