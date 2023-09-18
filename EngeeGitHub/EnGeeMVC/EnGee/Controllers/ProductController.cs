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
        //以下為樹傑的Action

        EngeeContext db = new EngeeContext();
        private IQueryable<TProduct> ApplyFilters(SSJ_ProductPageViewModel vm, IQueryable<TProduct> query)
        {//filters邏輯使用方法
            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                query = query.Where(p => p.ProductName.Contains(vm.txtKeyword));
            }
            if (vm.MainCategoryId.HasValue && vm.SubCategoryId.HasValue)
            {
                query = query.Where(p => p.MainCategoryId == vm.MainCategoryId.Value && p.SubcategoryId == vm.SubCategoryId.Value);
            }
            if (vm.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == vm.BrandId.Value);
            }
            return query;
        }
        public IActionResult IndexSSJ(SSJ_ProductPageViewModel vm, int page = 1, int pageSize = 6)
        {
            IQueryable<TProduct> query = db.TProducts;
            query = ApplyFilters(vm, query);
            var products = query.Select(p => new SSJ_ProductViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescribe = p.ProductDescribe,
                ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                ProductUnitPoint = p.ProductUnitPoint
            });

            // Pagination
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            var model = new SSJ_ProductPageViewModel
            {
                Products = products.ToList(),
                CurrentPage = page,
                TotalPages = totalPages,
                txtKeyword = vm.txtKeyword,
                MainCategoryId = vm.MainCategoryId,
                SubCategoryId = vm.SubCategoryId,
                BrandId = vm.BrandId
            };
            return View(model);
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
        //以下好像用不到
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult Create(TProduct p)
        //{
        //    EngeeContext db = new EngeeContext();
        //    db.TProducts.Add(p);
        //    db.SaveChanges();
        //    return RedirectToAction("IndexSSJ");
        //}
    }
}

