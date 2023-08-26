
using Microsoft.AspNetCore.Mvc;
using EnGee.Models;
using EnGee.ViewModel;
using System.Text.Json;

namespace EnGee.Controllers
{
    public class ShoppingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {
            EngeeContext db = new EngeeContext();
            var datas = from p in db.TProducts select p;
            List<SSJ_CProductWrap> list = new List<SSJ_CProductWrap>();
            foreach (var t in datas)
            {
                SSJ_CProductWrap w = new SSJ_CProductWrap();
                w.product = t;
                list.Add(w);
            }
            return View(list);
        }
        public IActionResult CartView()
        {
            if (!HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
                return RedirectToAction("List");

            string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
            List<SSJ_CShoppingCarItem> cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json) ?? new List<SSJ_CShoppingCarItem>(); // <-- 預防null值
            if (!cart.Any())
                return RedirectToAction("List");

            return View(cart);
        }
        public ActionResult AddToCart(int? id)
        {

            ViewBag.ProductId = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddToCart(SSJ_CAddToCartViewModel vm)
        {
            EngeeContext db = new EngeeContext();
            TProduct p = db.TProducts.FirstOrDefault(t => t.ProductId == vm.txtProductId);

            if (p == null)
            {
                return RedirectToAction("List");
            }
            string json = "";
            List<SSJ_CShoppingCarItem> cart = null;
            if (HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json);
            }
            else
            {
                cart = new List<SSJ_CShoppingCarItem>();
            }
            SSJ_CShoppingCarItem item = new SSJ_CShoppingCarItem();
            item.point = (int)p.ProductUnitPoint;
            item.ProductId = vm.txtProductId;
            item.count = vm.txtCount;
            item.tproduct = p;
            item.ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}";
            cart.Add(item);
            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
            return RedirectToAction("List");
        }
    }
}



