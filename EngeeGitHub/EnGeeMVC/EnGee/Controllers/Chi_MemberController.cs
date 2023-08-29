using EnGee.Data;
using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.ViewModels;
using prjMvcCoreDemo.Models;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class Chi_MemberController : Controller
    {
        
        public IActionResult List(CKeywordViewModel vm)
        {

            EngeeContext db = new EngeeContext();
            IEnumerable<TMember> datas = null;
            if (string.IsNullOrEmpty(vm.txtKeyword))
                datas = from p in db.TMembers
                        select p;
            else
                datas = db.TMembers.Where(t => t.Username.Contains(vm.txtKeyword)
                || t.Fullname.Contains(vm.txtKeyword)
                || t.Nickname.Contains(vm.txtKeyword)
                || t.Phone.Contains(vm.txtKeyword)
                || t.Address.Contains(vm.txtKeyword));
            return View(datas);
        }
        
        public IActionResult UserProfile()
        {

            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                using (var dbContext = new EngeeContext())
                {
                    TMember userFromDatabase = dbContext.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
                    if (userFromDatabase != null)
                    {
                        return View(userFromDatabase);
                    }
                }
            }
            
            return RedirectToAction("LoginLayout","Home"); // 用户未登录或找不到用户信息时重定向到登录页面
        }
    

    public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            EngeeContext db = new EngeeContext();
            TMember cust = db.TMembers.FirstOrDefault(t => t.MemberId == id);
            if (cust == null)
                return RedirectToAction("List");
            return View(cust);
        }
        [HttpPost]
        public IActionResult Edit(TMember custIn)
        {
            EngeeContext db = new EngeeContext();
            TMember custDb = db.TMembers.FirstOrDefault(t => t.MemberId == custIn.MemberId);

            if (custDb != null)
            {
                custDb.Nickname = custIn.Nickname;
                custDb.Address = custIn.Address;
                custDb.Fullname = custIn.Fullname;
                custDb.Phone = custIn.Phone;
                custDb.Password = custIn.Password;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
