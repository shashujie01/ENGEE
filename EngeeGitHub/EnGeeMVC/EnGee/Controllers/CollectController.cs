using EnGee.Models;
using EnGee.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EnGee.Controllers
{
    public class CollectController : Controller
    {
        private IWebHostEnvironment _enviro = null;
        public CollectController(IWebHostEnvironment c)
        {
            _enviro = c;
        }

        private EngeeContext db = new EngeeContext();


        //  文章管理
        public IActionResult Management()
        {
            IEnumerable<Rong_CollectManagementViewModel> collectmanage =
                from co in db.TCollects
                join m in db.TMembers on co.MemberId equals m.MemberId into memberJoin
                from m in memberJoin.DefaultIfEmpty() // Left outer join 之後要刪掉
                join d in db.TDeliveryTypes on co.DeliveryTypeId equals d.DeliveryTypeId
                select new Rong_CollectManagementViewModel
                {
                    CollectId = co.CollectId,
                    MemberId = m != null ? m.MemberId : 0,  // 之後要改成MemberId = m.MemberId
                    CollectTitle = co.CollectTitle,
                    CollectStartDate = co.CollectStartDate.ToString("yyyy/MM/dd"),
                    CollectEndDate = co.CollectEndDate.ToString("yyyy/MM/dd"),
                    DeliveryType = d.DeliveryType,
                    CollectStatus = co.CollectStatus
                };
            return View(collectmanage);
        }

        // 商品首頁
        public IActionResult CollectIndex()
        {
            IEnumerable<Rong_CollectIndexViewModel> collectindex =
                from co in db.TCollects
                join m in db.TMembers on co.MemberId equals m.MemberId into memberJoin
                from m in memberJoin.DefaultIfEmpty()
                join d in db.TDeliveryTypes on co.DeliveryTypeId equals d.DeliveryTypeId
                select new Rong_CollectIndexViewModel
                {
                    CollectId = co.CollectId,
                    Nickname = m != null ? m.Nickname : "沒有資料",
                    CollectTitle = co.CollectTitle,
                    CollectStartDate = co.CollectStartDate.ToString("yyyy/MM/dd"),
                    CollectEndDate = co.CollectEndDate.ToString("yyyy/MM/dd"),
                    CollectImagePath = co.CollectImagePath
                };

            return View(collectindex);

        }

        public IActionResult CollectInfomation(int? id)
        {
            if (id == null)
                return RedirectToAction("CollectIndex");
            var cinfo =
                (from c in db.TCollects
                 join d in db.TDeliveryTypes on c.DeliveryTypeId equals d.DeliveryTypeId
                 join m in db.TMembers on c.MemberId equals m.MemberId into memberJoin
                 from m in memberJoin.DefaultIfEmpty()
                 where c.CollectId == id
                 select new Rong_CollectInfomationViewModel
                 {
                     CollectId = c.CollectId,
                     Nickname = m != null ? m.Nickname : "沒有資料",
                     CollectTitle = c.CollectTitle,
                     CollectCaption = c.CollectCaption,
                     CollectStartDate = c.CollectStartDate.ToString("yyyy/MM/dd"),
                     CollectEndDate = c.CollectEndDate.ToString("yyyy/MM/dd"),
                     DeliveryTypeId = c.DeliveryTypeId,
                     DeliveryType = d.DeliveryType,
                     DeliveryAddress = c.DeliveryAddress != null ? c.DeliveryAddress : "",
                     ConvenienNum = c.ConvenienNum != null ? c.ConvenienNum : "",
                     CollectImagePath = c.CollectImagePath,
                     CollectItemName = c.CollectItemName,
                     CollectAmount = c.CollectAmount
                 })
                .FirstOrDefault();

            return View(cinfo);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Rong_CollectImageViewModel ci)
        {
            if (ci.photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string path = _enviro.WebRootPath + "/images/donation/" + photoName;
                ci.photo.CopyTo(new FileStream(path, FileMode.Create));
                ci.CollectImagePath = photoName;
            }
            
            TCollect c = new TCollect()
            {
                MemberId = ci.MemberId,
                CollectTitle = ci.CollectTitle,
                CollectCaption = ci.CollectCaption,
                CollectStartDate = ci.CollectStartDate,
                CollectEndDate = ci.CollectEndDate,
                DeliveryTypeId = ci.DeliveryTypeId,
                DeliveryAddress = ci.DeliveryAddress,
                ConvenienNum = ci.ConvenienNum,
                CollectStatus = ci.CollectStatus,
                CollectImagePath = ci.CollectImagePath,
                CollectItemName = ci.CollectItemName,
                MainCategoryId = ci.MainCategoryId,
                SubcategoryId = ci.SubcategoryId,
                CollectAmount = ci.CollectAmount
            };
            db.TCollects.Add(c);
            db.SaveChanges();
            return RedirectToAction("CollectInfomation");


            //var collect = new TCollect
            //{
            //    CollectTitle = cp.CollectTitle,
            //    CollectCaption = cp.CollectCaption,
            //    CollectEndDate = cp.CollectEndDate,
            //    //CollectImagePath = cp.CollectImagePath,
            //    DeliveryTypeId = cp.DeliveryTypeId,
            //    DeliveryAddress = cp.DeliveryAddress,
            //    ConvenienNum = cp.ConvenienNum,
            //    CollectStatus = cp.CollectStatus,
            //};
            //db.TCollects.Add(collect);
            //db.SaveChanges();
            ////var collectimg = new TCollectImage{ };
            //var collectItem = new TCollectItem
            //{
            //    CollectId = collect.CollectId,
            //    MainCategoryId = cp.MainCategoryId,
            //    SubcategoryId = cp.SubcategoryId,
            //    CollectItemName = cp.CollectItemName,
            //    CollectAmount = cp.CollectAmount
            //};

            //collect.collectItems.Add(collectItem);
            //db.SaveChanges();
        }


        public IActionResult DeleteCollectManagement(int? id)
        {
            if (id == null)
                return RedirectToAction("Management");
            TCollect c = db.TCollects.FirstOrDefault(t => t.CollectId == id);
            if (c != null)
            {
                db.TCollects.Remove(c);
                db.SaveChanges();
            }
            return RedirectToAction("Management");
        }
        
        public IActionResult DeleteCollectMember(int? id)
        {
            if (id == null) 
                return RedirectToAction("Create");

            TCollect c = db.TCollects.FirstOrDefault(t => t.CollectId == id);

            if (c != null)
            {
                db.TCollects.Remove(c);
                db.SaveChanges();
            }
            return RedirectToAction("CollectIndex");
        }

        public IActionResult EditCollect(int? id)
        {
            if (id == null)
                return RedirectToAction("CollectIndex");
            TCollect c = db.TCollects.FirstOrDefault(t => t.CollectId == id);
            if (c == null)
                return RedirectToAction("CollectIndex");
            Rong_CCollectWrap cWrap = new Rong_CCollectWrap();
            cWrap.coll = c;
            return View(cWrap);
        }
        [HttpPost]
        public IActionResult EditCollect(Rong_CCollectWrap cIn)
        {
            TCollect cDb = db.TCollects.FirstOrDefault(t => t.CollectId == cIn.CollectId);

            if (cDb != null)
            {
                if (cIn.photo != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    string path = _enviro.WebRootPath + "/images/donation/" + photoName;
                    cIn.photo.CopyTo(new FileStream(path, FileMode.Create));
                    cIn.CollectImagePath = photoName;
                }

                cDb.CollectTitle = cIn.CollectTitle;
                cDb.CollectCaption = cIn.CollectCaption;
                cDb.CollectEndDate = cIn.CollectEndDate;
                cDb.DeliveryTypeId = cIn.DeliveryTypeId;
                cDb.DeliveryAddress = cIn.DeliveryAddress;
                cDb.ConvenienNum = cIn.ConvenienNum;
                //cDb.CollectStatus = cIn.CollectStatus;
                cDb.CollectImagePath = cIn.CollectImagePath;
                cDb.MainCategoryId = cIn.MainCategoryId;
                cDb.SubcategoryId = cIn.SubcategoryId;
                cDb.CollectAmount = cIn.CollectAmount;
                db.SaveChanges();
            }
            return RedirectToAction("CollectInfomation");
        }


    }
}
