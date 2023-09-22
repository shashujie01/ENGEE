using Microsoft.AspNetCore.Mvc;
using EnGee.ViewModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using prjMvcCoreDemo.Models;
using EnGee.Models;

namespace EnGee.Controllers
{
    public class JING_DemandController : SuperController
    {
        private readonly EngeeContext _db;

        public JING_DemandController(EngeeContext db, CHI_CUserViewModel userViewModel) : base(userViewModel)
        {
            _db = db;////20230919合併出錯時加入
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
        private List<JING_CDemandCartItem> GetCartItems()
        {
            if (!HttpContext.Session.Keys.Contains(JING_CDictionary.SK_REQUESTED_DEMAND_LIST))
            {
                return new List<JING_CDemandCartItem>();
            }
            string json = HttpContext.Session.GetString(JING_CDictionary.SK_REQUESTED_DEMAND_LIST);
            return JsonSerializer.Deserialize<List<JING_CDemandCartItem>>(json) ?? new List<JING_CDemandCartItem>();
        }

        private void SaveCartItems(List<JING_CDemandCartItem> cart)
        {
            string json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(JING_CDictionary.SK_REQUESTED_DEMAND_LIST, json);
        }

        private int GetDeliveryFee(int deliveryTypeId)
        {
            using (var db = new EngeeContext())
            {
                var deliveryType = db.TDeliveryTypes.FirstOrDefault(dt => dt.DeliveryTypeId == deliveryTypeId);
                if (deliveryType != null)
                {
                    return (int)deliveryType.DeliveryFee;
                }
                // Default value if the delivery type is not found
                return 0;
            }
        }

        private List<JING_CDemandCartItem> GetOrUpdateCart(int productId, int count, int deliveryOption)
        {
            // 使用 GetCartItems 方法獲取購物車項目
            var cart = GetCartItems();
            JING_CDemandCartItem existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == productId);
            if (p != null)
            {
                if (existingItem != null)
                {// 如果商品已存在，更新數量
                    existingItem.count += count;
                }
                else
                {// 如果商品不存在，創建新項目並添加到購物車
                    JING_CDemandCartItem item = new JING_CDemandCartItem
                    {
                        point = (int)p.ProductUnitPoint,
                        ProductId = productId,
                        count = count,
                        tproduct = p,
                        ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                        DeliveryTypeID = deliveryOption
                    };
                    cart.Add(item);
                }
                // 保存更新後的購物車到 Session
                string json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(JING_CDictionary.SK_REQUESTED_DEMAND_LIST, json);
            }
            return cart;
        }

        public IActionResult DemandView()
        {
            TMember loggedInUser = GetLoggedInUser();
            TMember userFromDatabase = _db.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
            ViewBag.userFromDatabase = userFromDatabase;
            List<JING_CDemandCartItem> cart = GetCartItems();
            return View(cart);
        }

        [HttpPost]
        //Details會post商品資料過來
        public ActionResult DemandView(JING_CAddToCartViewModel vm, int? deliverytypeid)
        {
            if (deliverytypeid.HasValue)
            {
                HttpContext.Session.SetInt32("DeliveryType", deliverytypeid.Value);//TODO用處?
            }
            if (vm == null)
            { return RedirectToAction("DemandView"); }

            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == vm.txtProductId);
            if (p == null)
            {
                return RedirectToAction("DemandView");
            }
            List<JING_CDemandCartItem> cart = GetCartItems();
            JING_CDemandCartItem existingItem = cart.FirstOrDefault(i => i.ProductId == vm.txtProductId);
            if (existingItem != null)
            {
                existingItem.count += vm.txtCount;
            }
            else
            {
                cart.Add(new JING_CDemandCartItem                                                     
                {
                    point = (int)p.ProductUnitPoint,
                    ProductId = vm.txtProductId,
                    count = vm.txtCount,
                    tproduct = p,
                    ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                    DeliveryTypeID = (int)deliverytypeid
                });
            }
            SaveCartItems(cart);
            return RedirectToAction("DemandView");
        }
        //------------------------
        [HttpGet]
        public ActionResult QuickAddToCart(int productId)
        {
            int defaultCount = 1;
            int defaultDeliveryOption = 1;
            HttpContext.Session.SetInt32("DeliveryType", defaultDeliveryOption);
            GetOrUpdateCart(productId, defaultCount, defaultDeliveryOption);
            return RedirectToAction("DemandView");
        }

        [HttpPost]
        public JsonResult AddToCartWithAjax(JING_CAddToCartViewModel vm, int deliveryOption)
        {
            if (vm == null)
            {
                return Json(new { success = false, message = "加入購物車失敗" });
            }
            GetOrUpdateCart(vm.txtProductId, vm.txtCount, deliveryOption);
            return Json(new { success = true, message = "已加入購物車" });
        }

        [HttpPost]
        public ActionResult ConfirmPurchase(JING_ConfirmPurchaseViewModel vm, string SelectedProducts)
        { //確認結帳傳至後端
            var cart = GetCartItems();
            if (cart == null || cart.Count == 0) return RedirectToAction("CartView");// 檢查購物車中是否有商品
            int? sessionDeliveryType = HttpContext.Session.GetInt32("DeliveryType");
            if (sessionDeliveryType.HasValue)
            {
                vm.DeliveryType = sessionDeliveryType.Value;
            }
            // 將字符串反序列化為整數列表，Session中獲取CHKBox被選中的ID
            List<int> selectedProductIds = JsonSerializer.Deserialize<List<int>>(SelectedProducts);
            // 從Session中獲取購物車商品
            string json = HttpContext.Session.GetString(JING_CDictionary.SK_REQUESTED_DEMAND_LIST);
            cart = JsonSerializer.Deserialize<List<JING_CDemandCartItem>>(json);
            // 如果購物車為空，則重新導向到購物車視圖
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("CartView");
            }
            // 創建新的訂單並儲存
            int totalUsagePoints = 0;
            using (EngeeContext db = new EngeeContext())
            {
                TOrder order = new TOrder
                {
                    OrderDate = DateTime.Now,
                    //DeliveryTypeId = vm.DeliveryType,
                    //DeliveryAddress = vm.DeliveryAddress,
                    BuyerId = 59,//TODO:尚未串接
                    //SellerId = 60,//尚未串接
                    OrderStatus = "3",//結帳後為3
                    OrderCatagory = 1,//買賣為1
                    ConvienenNum = "0",//超商尚未串接
                    DeliveryFee = GetDeliveryFee(vm.DeliveryType)//用函式尋找DeliveryFee
                };
                db.TOrders.Add(order);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    var innerException = e.InnerException;
                    while (innerException != null)
                    {
                        Console.WriteLine(innerException.Message);
                        innerException = innerException.InnerException;
                    }
                }
                // 對於購物車中的每一項商品，都在TOrderDetail表中新增一個詳細資料行
                foreach (var item in cart)
                {
                    if (selectedProductIds.Contains(item.ProductId))//chkbox選中的ID是否包含item.ProductId
                    {
                        TOrderDetail orderDetail = new TOrderDetail
                        {
                            OrderId = order.OrderId,
                            OrderDate = order.OrderDate,
                            ProductId = item.ProductId,
                            ProductUnitPoint = item.point,
                            OrderQuantity = item.count,
                            //BuyerId = order.BuyerId,
                            //SellerId = order.SellerId
                        };
                        db.TOrderDetails.Add(orderDetail);
                        totalUsagePoints += item.小計;// 計算小計
                    }
                }
                order.OrderTotalUsagePoints = totalUsagePoints;
                db.SaveChanges();

                // 購買後從Session中清除購物車
                cart.RemoveAll(item => selectedProductIds.Contains(item.ProductId));
                json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
                //HttpContext.Session.Remove(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
                //HttpContext.Session.Remove("DeliveryType");//清除寄送方式Session
                //TODO寄送方式是否跟別的一起傳
            }
            return RedirectToAction("DemandView");
        }

        [HttpPost]
        public JsonResult UpdateCartItem(int productId, int newCount)
        {//更新小計、totalPoint用
            // 從Session中獲取購物車列表
            var cart = GetCartItems();
            // 在購物車中尋找與指定ID匹配的商品
            var itemToUpdate = cart.FirstOrDefault(item => item.ProductId == productId);
            // 如果找不到該商品，返回失敗
            if (itemToUpdate == null) return Json(new { success = false, message = "Product not found" });
            // 更新商品數量
            itemToUpdate.count = newCount;
            // 保存新的購物車列表到Session
            SaveCartItems(cart);
            // 返回成功和更新後的資訊
            return Json(new { success = true, subtotalPoints = itemToUpdate.小計, itemToUpdate.count, totalPoints = cart.Sum(item => item.小計) });
        }
        [HttpPost]
        public JsonResult DeleteCartItem(int productId)
        {
            // 從Session中獲取購物車列表
            var cart = GetCartItems();
            // 在購物車中尋找與指定ID匹配的商品
            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);
            // 如果找不到該商品，返回失敗
            if (itemToRemove == null) return Json(new { success = false, message = "未找到商品" });
            // 從購物車中移除該商品
            cart.Remove(itemToRemove);
            // 保存新的購物車列表到Session
            SaveCartItems(cart);
            // 返回成功
            return Json(new { success = true });
        }
    }
}