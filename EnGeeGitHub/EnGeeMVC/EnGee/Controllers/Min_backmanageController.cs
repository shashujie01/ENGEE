using EnGee.Data;
using Microsoft.AspNetCore.Mvc;
using EnGee.Models;

namespace EnGee.Controllers
{
    public class Min_backmanageController : Controller
    {
        public IActionResult List()
        {
            using (var db = new EngeeContext())
            {
                var data = db.TMembers.ToList();
                return View(data);
            }
        }
        public IActionResult Delete(int? id)
        {
            if(id== null)
            {
                return RedirectToAction("List");
            }
            else
            {
                using (var db = new EngeeContext())
                {
                   var checkdata= db.TMembers.FirstOrDefault(t=>t.MemberId==id);
                    if(checkdata!= null)
                    {
                        db.TMembers.Remove(checkdata);
                        db.SaveChanges();
                    }
                    return RedirectToAction("List");
                }
            }  
                return View();
        }
        public IActionResult Edit(int?id)
        {
            if (id == null)
            {
                return RedirectToAction("List");
            }
            else
            {
                using (var db = new EngeeContext())
                {
                    var checkDbid = db.TMembers.FirstOrDefault(t=>t.MemberId==id);
                    if(checkDbid != null)
                    {
                        return View(checkDbid);
                    }
                    return RedirectToAction("List");
                }
            }
        }

        [HttpPost]
        public IActionResult Edit(TMember model)
        {
            using(var db = new EngeeContext())
            {
                var checkDbId=db.TMembers.FirstOrDefault(t=>t.MemberId == model.MemberId);
                if (checkDbId != null)
                {
                    checkDbId.MemberId=model.MemberId;
                    checkDbId.Username=model.Username;
                    checkDbId.Password = model.Password;
                    checkDbId.RePassword = model.RePassword;
                    checkDbId.Email = model.Email;
                    checkDbId.Fullname = model.Fullname;
                    checkDbId.Gender = model.Gender;
                    checkDbId.Address = model.Address;
                    checkDbId.Phone = model.Phone;
                    checkDbId.Birth = model.Birth;
                    checkDbId.PhotoPath = model.PhotoPath;
                    checkDbId.Introduction = model.Introduction;



                    db.SaveChanges();
                }
                return RedirectToAction("List");
            }
        }
    }
}
