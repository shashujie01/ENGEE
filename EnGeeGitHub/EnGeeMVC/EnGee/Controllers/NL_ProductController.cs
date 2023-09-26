using EnGee.Models;
using EnGee.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.Models;
using prjMvcCoreDemo.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class NL_ProductController : SuperController
    {
        private IWebHostEnvironment _enviro = null;
        public NL_ProductController(IWebHostEnvironment p, CHI_CUserViewModel userViewModel) : base(userViewModel)
        {//20230919合併出錯時加入
            _enviro = p;
        }

        private TMember GetLoggedInUser()
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return JsonSerializer.Deserialize<TMember>(userJson);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(NL_CKeywordViewModel vm, int page = 1)
        {
            int pageSize = 12; // 每頁顯示的數量
            EngeeContext db = new EngeeContext();
            TMember loggedInUser = GetLoggedInUser();
            var userId = loggedInUser.MemberId;


            IQueryable<TProduct> datas = from p in db.TProducts
                                         select p;

            IQueryable<TProduct> productsQuery = datas.Include(p => p.Brand)
                                                    .Include(p => p.MainCategory)
                                                    .Include(p => p.Subcategory)
                                                    .Where(p => p.SellerId == userId);

            if (!string.IsNullOrEmpty(vm.txtKeyword) && !string.IsNullOrEmpty(vm.searchBy))
            {
                if (vm.searchBy == "Name")
                {
                    productsQuery = productsQuery.Where(p => p.ProductName.Contains(vm.txtKeyword));
                }
                else if (vm.searchBy == "Point")
                {
                    decimal keywordAsDecimal;
                    if (decimal.TryParse(vm.txtKeyword, out keywordAsDecimal))
                    {
                        productsQuery = productsQuery.Where(p => p.ProductUnitPoint == keywordAsDecimal);
                    }
                }
                else if (vm.searchBy == "BrandId")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.BrandId == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "MainCategoryId")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.MainCategoryId == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "DeliveryTypeId")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.DeliveryTypeId == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "DonationStatus")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.DonationStatus == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "ProductSaleStatus")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.ProductSaleStatus == keywordAsInt);
                    }
                }
            }

            var totalProducts = await productsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var filteredProducts = await productsQuery.Skip((page - 1) * pageSize)
                                                    .Take(pageSize)
                                                    .ToListAsync();

            List<NL_ProductModel> list = new List<NL_ProductModel>();
            foreach (var t in filteredProducts)
            {
                NL_ProductModel w = new NL_ProductModel();
                w.Product = t;
                w.Brand = t.Brand;
                w.MainCategory = t.MainCategory;
                w.Subcategory = t.Subcategory;
                list.Add(w);
            }

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = vm.txtKeyword;
            ViewBag.SearchBy = vm.searchBy;

            var brandData = db.TBrands.ToList();
            ViewBag.BrandId = new SelectList(brandData, "BrandId", "BrandCategory");

            var mainCategoryData = db.TCosmeticMainCategories.ToList();
            ViewBag.MainCategoryId = new SelectList(mainCategoryData, "MainCategoryId", "MainCategory");

            var deliveryTypeData = db.TDeliveryTypes.ToList();
            ViewBag.DeliveryTypeId = new SelectList(deliveryTypeData, "DeliveryTypeId", "DeliveryType");


            return View(list);
        }

        public IActionResult Add()
        {
            EngeeContext db = new EngeeContext();

            var brandCategories = db.TBrands.ToList();
            var MainCategory = db.TCosmeticMainCategories.ToList();
            var Subcategory = db.TCosmeticSubcategories.ToList();
            var DeliveryType = db.TDeliveryTypes.ToList();

            ViewBag.BrandCategories = brandCategories;
            ViewBag.MainCategory = MainCategory;
            ViewBag.Subcategory = Subcategory;
            ViewBag.TDeliveryType = DeliveryType;

            TMember loggedInUser = GetLoggedInUser();
            ViewBag.userId = loggedInUser.MemberId;

            return View();
        }
        [HttpPost]
        public IActionResult Add(NL_ProductModel p, IFormFile photo)
        {
            EngeeContext db = new EngeeContext();

            if (photo != null && photo.Length > 0)
            {
                string originalFileName = Path.GetFileName(photo.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                string photoName = Guid.NewGuid().ToString() + fileExtension;
                string path = _enviro.WebRootPath + "/images/ProductImages/" + photoName;
                p.photo.CopyTo(new FileStream(path, FileMode.Create));
                p.ProductImagePath = photoName;
            }
            db.TProducts.Add(p.Product);
            db.SaveChanges();            
            return RedirectToAction("Index");
        }

        public IActionResult AddOne()
        {
            EngeeContext db = new EngeeContext();

            var brandCategories = db.TBrands.ToList();
            var MainCategory = db.TCosmeticMainCategories.ToList();
            var Subcategory = db.TCosmeticSubcategories.ToList();
            var DeliveryType = db.TDeliveryTypes.ToList();

            ViewBag.BrandCategories = brandCategories;
            ViewBag.MainCategory = MainCategory;
            ViewBag.Subcategory = Subcategory;
            ViewBag.TDeliveryType = DeliveryType;

            TMember loggedInUser = GetLoggedInUser();
            ViewBag.userId = loggedInUser.MemberId;

            return View();
        }

        [HttpPost]
        public IActionResult AddOne(NL_ProductModel p, IFormFile photo)
        {
            EngeeContext db = new EngeeContext();

            if (photo != null && photo.Length > 0)
            {
                string originalFileName = Path.GetFileName(photo.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                string photoName = Guid.NewGuid().ToString() + fileExtension;
                string path = _enviro.WebRootPath + "/images/ProductImages/" + photoName;
                p.photo.CopyTo(new FileStream(path, FileMode.Create));
                p.ProductImagePath = photoName;
            }
            db.TProducts.Add(p.Product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            EngeeContext db = new EngeeContext();
            TProduct prod = db.TProducts.FirstOrDefault(t => t.ProductId == id);
            if (prod != null)
            {
                db.TProducts.Remove(prod);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            EngeeContext db = new EngeeContext();

            var brandCategories = db.TBrands.ToList();
            var MainCategory = db.TCosmeticMainCategories.ToList();
            var Subcategory = db.TCosmeticSubcategories.ToList();
            var DeliveryType = db.TDeliveryTypes.ToList();

            ViewBag.BrandCategories = brandCategories;
            ViewBag.MainCategory = MainCategory;
            ViewBag.Subcategory = Subcategory;
            ViewBag.TDeliveryType = DeliveryType;

            TProduct prod = db.TProducts.FirstOrDefault(t => t.ProductId == id);
            if (prod == null)
                return RedirectToAction("List");
            NL_ProductModel NLPM = new NL_ProductModel();
            NLPM.Product = prod;
            return View(NLPM);
        }
        [HttpPost]
        public IActionResult Edit(NL_ProductModel prodIn, IFormFile photo)
        {
            EngeeContext db = new EngeeContext();
            TProduct prodDb = db.TProducts.FirstOrDefault(t => t.ProductId == prodIn.ProductId);

            if (prodDb != null)
            {
                if (photo != null && photo.Length > 0)
                {
                    string originalFileName = Path.GetFileName(photo.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);

                    string photoName = Guid.NewGuid().ToString() + fileExtension;
                    string path = _enviro.WebRootPath + "/images/ProductImages/" + photoName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        photo.CopyTo(stream);
                    }
                    prodDb.ProductImagePath = photoName;
                }

                prodDb.ProductName = prodIn.ProductName;
                prodDb.BrandId = prodIn.BrandId;
                prodDb.MainCategoryId = prodIn.MainCategoryId;
                prodDb.SubcategoryId = prodIn.SubcategoryId;
                prodDb.ProductDescribe = prodIn.ProductDescribe;
                prodDb.ProductUnitPoint = prodIn.ProductUnitPoint;
                prodDb.ProductRemainingQuantity = prodIn.ProductRemainingQuantity;
                prodDb.ProductExpirationDate = prodIn.ProductExpirationDate;
                prodDb.ProductUsageStatus = prodIn.ProductUsageStatus;
                prodDb.DonationStatus = prodIn.DonationStatus;
                prodDb.DateOfSale = prodIn.DateOfSale;
                prodDb.DeliveryTypeId = prodIn.DeliveryTypeId;
                prodDb.SellerId = prodIn.SellerId;
                prodDb.ProductSaleStatus = prodIn.ProductSaleStatus;

                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Sort(string sortingOption, NL_CKeywordViewModel vm, int page = 1)
        {
            int pageSize = 12; // 每頁顯示的數量
            EngeeContext db = new EngeeContext();
            TMember loggedInUser = GetLoggedInUser();
            var userId = loggedInUser.MemberId;

            IQueryable<TProduct> datas = from p in db.TProducts
                                         select p;

            IQueryable<TProduct> productsQuery = datas.Include(p => p.Brand)
                                                    .Include(p => p.MainCategory)
                                                    .Include(p => p.Subcategory)
                                                    .Where(p => p.SellerId == userId);

            if (!string.IsNullOrEmpty(vm.txtKeyword) && !string.IsNullOrEmpty(vm.searchBy))
            {
                if (vm.searchBy == "Name")
                {
                    productsQuery = productsQuery.Where(p => p.ProductName.Contains(vm.txtKeyword));
                }
                else if (vm.searchBy == "Point")
                {
                    decimal keywordAsDecimal;
                    if (decimal.TryParse(vm.txtKeyword, out keywordAsDecimal))
                    {
                        productsQuery = productsQuery.Where(p => p.ProductUnitPoint == keywordAsDecimal);
                    }
                }
                else if (vm.searchBy == "BrandId")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.BrandId == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "MainCategoryId")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.MainCategoryId == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "DeliveryTypeId")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.DeliveryTypeId == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "DonationStatus")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.DonationStatus == keywordAsInt);
                    }
                }
                else if (vm.searchBy == "ProductSaleStatus")
                {
                    int keywordAsInt;
                    if (int.TryParse(vm.txtKeyword, out keywordAsInt))
                    {
                        productsQuery = productsQuery.Where(p => p.ProductSaleStatus == keywordAsInt);
                    }
                }
            }

            // 執行排序操作
            if (sortingOption == "highToLow")
            {
                productsQuery = productsQuery.OrderByDescending(p => p.ProductUnitPoint);
            }
            else if (sortingOption == "lowToHigh")
            {
                productsQuery = productsQuery.OrderBy(p => p.ProductUnitPoint);
            }
            else if (sortingOption == "recentToOld")
            {
                productsQuery = productsQuery.OrderByDescending(p => p.DateOfSale);
            }
            else if (sortingOption == "oldToRecent")
            {
                productsQuery = productsQuery.OrderBy(p => p.DateOfSale);
            }

            var totalProducts = await productsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var filteredProducts = await productsQuery.Skip((page - 1) * pageSize)
                                                    .Take(pageSize)
                                                    .ToListAsync();

            List<NL_ProductModel> list = new List<NL_ProductModel>();
            foreach (var t in filteredProducts)
            {
                NL_ProductModel w = new NL_ProductModel();
                w.Product = t;
                w.Brand = t.Brand;
                w.MainCategory = t.MainCategory;
                w.Subcategory = t.Subcategory;
                list.Add(w);
            }

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = vm.txtKeyword;
            ViewBag.SearchBy = vm.searchBy;

            var brandData = db.TBrands.ToList();
            ViewBag.BrandId = new SelectList(brandData, "BrandId", "BrandCategory");

            var mainCategoryData = db.TCosmeticMainCategories.ToList();
            ViewBag.MainCategoryId = new SelectList(mainCategoryData, "MainCategoryId", "MainCategory");

            var deliveryTypeData = db.TDeliveryTypes.ToList();
            ViewBag.DeliveryTypeId = new SelectList(deliveryTypeData, "DeliveryTypeId", "DeliveryType");

            return View("List", list); // 將排序後的結果傳遞給View
        }

        [HttpGet]
        public IActionResult GetSubcategories(int mainCategoryId)
        {            
            EngeeContext db = new EngeeContext();
            var subcategories = db.TCosmeticSubcategories
                .Where(s => s.MainCategoryId == mainCategoryId)
                .ToDictionary(s => s.SubcategoryId, s => s.Subcategory);

            return Json(subcategories);
        }
    }
}
