using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EnGee.Controllers
{
    public class ProductController : Controller
    {
EngeeContext db = new EngeeContext();

        public IActionResult Index(int page = 1, int pageSize = 6)
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
                TotalPages = totalPages
            };

            return View("Index", model);  // 使用 Index 的 View 來呈現篩選結果
        }

        [HttpGet]
        public IActionResult FilterByBrandId(int BrandId, int page = 1, int pageSize = 6)
        {
            var products = db.TProducts.Where(p => p.BrandId == BrandId )
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
                TotalPages = totalPages
            };

            return View("Index", model);  
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
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");
            EngeeContext db = new EngeeContext();
            TProduct cust = db.TProducts.FirstOrDefault(t => t.ProductId== id);
            if (cust != null)
            {
                db.TProducts.Remove(cust);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
