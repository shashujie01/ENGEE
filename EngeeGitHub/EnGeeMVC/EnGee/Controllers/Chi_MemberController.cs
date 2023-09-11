using EnGee.Data;
using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.ViewModels;
using prjMvcCoreDemo.Models;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class Chi_MemberController : SuperController
    {
        
        public IActionResult List(CKeywordViewModel vm) //沒有畫面
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

            using (var dbContext = new EngeeContext())
            {
                // 這裡可以直接使用 Session 中的用戶資訊，無需再次驗證
                string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);

                TMember userFromDatabase = dbContext.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
                if (userFromDatabase != null)
                {
                    return View(userFromDatabase);
                }
            }

            // 如果未找到用戶或其他錯誤情況導向登錄頁面
            return RedirectToAction("LoginLayout", "Home");
        }
    

    public IActionResult Edit(int? id) //沒有畫面
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
