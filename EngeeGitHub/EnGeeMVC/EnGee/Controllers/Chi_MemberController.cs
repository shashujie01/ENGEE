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
        private IWebHostEnvironment _enviro = null;
        public Chi_MemberController(IWebHostEnvironment p)
        {
            _enviro = p;
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
                    CHI_CMemberWrap memberWrap = new CHI_CMemberWrap
                    {
                        member = userFromDatabase
                    };
                    return View(memberWrap);
                }
            }
            // 如果未找到用戶或其他錯誤情況導向登錄頁面
            return RedirectToAction("LoginLayout", "Home");
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("UserProfile");
            EngeeContext db = new EngeeContext();
            TMember mem = db.TMembers.FirstOrDefault(t => t.MemberId == id);
            if (mem == null)
                return RedirectToAction("UserProfile");
            CHI_CMemberWrap memWp = new CHI_CMemberWrap();
            memWp.member = mem;
            return View(memWp);
        }
        [HttpPost]
        public IActionResult Edit(CHI_CMemberWrap memIn)
        {
            EngeeContext db = new EngeeContext();
            TMember memDb = db.TMembers.FirstOrDefault(t => t.MemberId == memIn.MemberId);

            if (memDb != null)
            {
                if (memIn.photo != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    string path = _enviro.WebRootPath + "/images/UserImage/" + photoName;
                    memIn.photo.CopyTo(new FileStream(path, FileMode.Create));
                    memDb.PhotoPath = photoName;
                }

                memDb.Fullname = memIn.Fullname;
                memDb.Email= memIn.Email;
                memDb.Address=memIn.Address;
                memDb.Phone= memIn.Phone;

                

                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

    }
}



