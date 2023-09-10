using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEnGeeDemo.Models;
using prjEnGeeDemo.ViewModels;

namespace prjEnGeeDemo.Controllers
{
    public class ProductController : Controller
    {
        private IWebHostEnvironment _enviro = null;
        public ProductController(IWebHostEnvironment p)
        {
            _enviro = p;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(string txtKeyword, int page = 1)
        {
            int pageSize = 10; // 每頁顯示的數量
            EngeeContext db = new EngeeContext();
            IQueryable<TProduct> datas = from p in db.TProducts
                                         select p;

            IQueryable<TProduct> productsQuery = datas.Include(p => p.Brand)
                                                      .Include(p => p.MainCategory)
                                                      .Include(p => p.Subcategory);

            if (!string.IsNullOrEmpty(txtKeyword))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(txtKeyword));
            }

            var totalProducts = productsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var filteredProducts = productsQuery.Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();

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
            ViewBag.Keyword = txtKeyword;

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
                string path = _enviro.WebRootPath + "/images/product/" + photoName;
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
                string path = _enviro.WebRootPath + "/images/product/" + photoName;
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
                    string path = _enviro.WebRootPath + "/images/product/" + photoName;
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

        //以下為樹傑的Action

        EngeeContext db = new EngeeContext();
        public IActionResult IndexSSJ(int page = 1, int pageSize = 6)
        {
            var products = db.TProducts.Select(p => new SSJ_ProductViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescribe = p.ProductDescribe,
                ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                ProductUnitPoint = p.ProductUnitPoint
            });

            // Get the number of products to determine total pages
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            var model = new SSJ_ProductPageViewModel
            {
                Products = products.ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult FilterByCategoryId(int mainCategoryId, int subCategoryId, int page = 1, int pageSize = 6)
        {
            var products = db.TProducts.Where(p => p.MainCategoryId == mainCategoryId && p.SubcategoryId == subCategoryId)
                                        .Select(p => new SSJ_ProductViewModel
                                        {
                                            ProductId = p.ProductId,
                                            ProductName = p.ProductName,
                                            ProductDescribe = p.ProductDescribe,
                                            ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                                            ProductUnitPoint = p.ProductUnitPoint
                                        });

            // Get the number of products to determine total pages
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            var model = new SSJ_ProductPageViewModel
            {
                Products = products.ToList(),
                CurrentPage = page,
                TotalPages = totalPages,
                MainCategoryId = mainCategoryId,
                SubCategoryId = subCategoryId
            };

            return View("IndexSSJ", model);  // 使用 Index 的 View 來呈現篩選結果
        }

        [HttpGet]
        public IActionResult FilterByBrandId(int brandId, int page = 1, int pageSize = 6)
        {
            var products = db.TProducts.Where(p => p.BrandId == brandId)
                                        .Select(p => new SSJ_ProductViewModel
                                        {
                                            ProductId = p.ProductId,
                                            ProductName = p.ProductName,
                                            ProductDescribe = p.ProductDescribe,
                                            ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                                            ProductUnitPoint = p.ProductUnitPoint
                                        });

            // Get the number of products to determine total pages
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            var model = new SSJ_ProductPageViewModel
            {
                Products = products.ToList(),
                CurrentPage = page,
                TotalPages = totalPages,
                BrandId = brandId
            };

            return View("IndexSSJ", model);
        }

        public IActionResult Details(int id)
        {

            var product = db.TProducts
                            .Include(p => p.Brand)
                            .Include(p => p.MainCategory)
                            .Include(p => p.Subcategory)
                            .Include(p => p.DeliveryType)
                            .SingleOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            var PDviewModel = new SSJ_ProductDetailsViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                BrandName = product.Brand?.BrandName,
                MainCategoryName = product.MainCategory?.MainCategory,
                SubcategoryName = product.Subcategory?.Subcategory,
                ProductDescribe = product.ProductDescribe,
                ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                ProductUnitPoint = product.ProductUnitPoint,
                ProductRemainingQuantity = product.ProductRemainingQuantity,
                ProductExpirationDate = product.ProductExpirationDate,
                ProductUsageStatus = product.ProductUsageStatus,
                DonationStatus = product.DonationStatus,
                DateOfSale = product.DateOfSale,
                DeliveryTypeName = product.DeliveryType?.DeliveryType,
                DeliveryFee = product.DeliveryType?.DeliveryFee ?? 0,
                //SellerName = product.Seller?.MemberName,
                ProductSaleStatus = product.ProductSaleStatus,
            };

            return View(PDviewModel);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TProduct p)
        {
            EngeeContext db = new EngeeContext();
            db.TProducts.Add(p);
            db.SaveChanges();
            return RedirectToAction("IndexSSJ");
        }

        public IActionResult DeleteSSJ(int? id)
        {
            if (id == null)
                return RedirectToAction("IndexSSJ");
            EngeeContext db = new EngeeContext();
            TProduct cust = db.TProducts.FirstOrDefault(t => t.ProductId == id);
            if (cust != null)
            {
                db.TProducts.Remove(cust);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}

