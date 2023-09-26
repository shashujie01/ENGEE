using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.ViewModels;



namespace WebApplication2.Controllers
{


    public class MemberController : Controller
    {

        //驗證信箱
        private readonly IConfiguration _configuration;

        public MemberController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(CKeywordViewModel vm)
        {
            EngeeContext db = new EngeeContext();
            IEnumerable<TMember> datas = Enumerable.Empty<TMember>();
            if (string.IsNullOrEmpty(vm.txtKeyword))
                datas = from p in db.TMembers
                        select p;
            else
            {
                datas = db.TMembers.Where(t => t.Username.Contains(vm.txtKeyword)
                || t.Phone.Contains(vm.txtKeyword)
                || t.Email.Contains(vm.txtKeyword));
                //||t.Address.Contains(vm.TxtKeyword));
            }


            return View(datas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TMember p)
        {//0:null  1:女生  2:男生
            if (p.Gender == null)
            {

                p.Gender = 0;
            }

            EngeeContext db = new EngeeContext();
            db.TMembers.Add(p);
            db.SaveChanges();
            return RedirectToAction("List");




        }



        //public IActionResult CreateNextPage()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult CreateNextPage(TMember p)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // 保存到数据库
        //        EngeeContext db = new EngeeContext();
        //        db.TMembers.Add(p);
        //        db.SaveChanges();

        //        return RedirectToAction("List");


        //    }
        //    return View(p);

        //}
    }
}