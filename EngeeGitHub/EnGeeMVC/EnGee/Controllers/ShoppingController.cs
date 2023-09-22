using Microsoft.AspNetCore.Mvc;
using EnGee.Models;
using EnGee.ViewModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using prjMvcCoreDemo.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;


namespace EnGee.Controllers
{
    public class ShoppingController : SuperController
    {//TODO特定時間清除多餘SESSION
        private readonly EngeeContext _db;
        public ShoppingController(EngeeContext db, CHI_CUserViewModel userViewModel) : base(userViewModel)
        {
            _db = db;
        }
        //private readonly IDbConnection _db;

        //public ShoppingController(IConfiguration configuration, CHI_CUserViewModel userViewModel) : base(userViewModel)
        //{
        //    _db = new SqlConnection(configuration.GetConnectionString("EnGeeContextConnection"));
        //}

        private TMember GetLoggedInUser()
        { // 從Session獲取當前登錄用戶
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return JsonSerializer.Deserialize<TMember>(userJson);
        }

        private List<SSJ_CShoppingCarItem> GetCartItems()//TODO存入不同的cart
        {// 從Session獲取購物車項目
            if (!HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST))
            {
                return new List<SSJ_CShoppingCarItem>();
            }
            string json = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST);
            return JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json) ?? new List<SSJ_CShoppingCarItem>();
        }

        private void SaveCartItems(List<SSJ_CShoppingCarItem> cart)
        { // 將購物車項目保存到Session
            string json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
        }
        //---------超商取貨購物車------------------
        private List<SSJ_CShoppingCarItem> GetCartItems_ConvenienceStore()
        {
            if (!HttpContext.Session.Keys.Contains(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST_CONVENIENCE_STORE))
            {
                return new List<SSJ_CShoppingCarItem>();
            }
            string json_ConvenienceStore = HttpContext.Session.GetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST_CONVENIENCE_STORE);
            return JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(json_ConvenienceStore) ?? new List<SSJ_CShoppingCarItem>();
        }

        private void SaveCartItems_ConvenienceStore(List<SSJ_CShoppingCarItem> cart_ConvenienceStore)
        {
            string json_ConvenienceStore = JsonSerializer.Serialize(cart_ConvenienceStore);
            HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST_CONVENIENCE_STORE, json_ConvenienceStore);
        }
        //---------超商取貨購物車------------------

        private int GetDeliveryFee(int deliveryTypeId)
        { // 根據送貨類型獲取運費
            using (var db = new EngeeContext())
            {
                var deliveryType = db.TDeliveryTypes.FirstOrDefault(dt => dt.DeliveryTypeId == deliveryTypeId);
                if (deliveryType != null)
                {
                    return (int)deliveryType.DeliveryFee;
                }
                // 若找不到送貨類型，則返回預設值
                return 0;
            }
        }

        private List<SSJ_CShoppingCarItem> GetAndUpdateCart(int productId, int count, int deliveryOption)
        { // 獲取或更新購物車資料

            var cart = GetCartItems();// 使用 GetCartItems 方法獲取購物車項目
            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == productId);
            if (p != null)
            {
                if (existingItem != null)
                {// 如果商品已存在，更新數量
                    existingItem.count += count;
                }
                else
                {// 如果商品不存在，創建新項目並添加到購物車
                    SSJ_CShoppingCarItem item = new SSJ_CShoppingCarItem
                    {
                        ProductId = productId,
                        SellerId = p.SellerId,
                        count = count,
                        point = (int)p.ProductUnitPoint,
                        ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                        DeliveryTypeID = deliveryOption,
                        tproduct = p
                    };
                    cart.Add(item);
                }
                // 保存更新後的購物車到 Session
                string json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
            }
            return cart;
        }

        private void LogDbUpdateException(DbUpdateException e)
        {
            var innerException = e.InnerException;
            while (innerException != null)
            {
                Console.WriteLine(innerException.Message);
                innerException = innerException.InnerException;
            }
        }
        private void HandleCart(TProduct product, int productId, int count, int deliveryTypeId,
    Func<List<SSJ_CShoppingCarItem>> getCartFunc, Action<List<SSJ_CShoppingCarItem>> saveCartFunc)
        {//加入不同購物車
            //TODO可能要分加入購物車與直接購買
            List<SSJ_CShoppingCarItem> cart = getCartFunc();
            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.count += count;
            }
            else
            {
                cart.Add(new SSJ_CShoppingCarItem
                {
                    tproduct = product,
                    SellerId = product.SellerId,
                    point = (int)product.ProductUnitPoint,
                    ProductId = productId,
                    count = count,
                    ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                    DeliveryTypeID = deliveryTypeId
                });
            }
            saveCartFunc(cart);
        }
        //------------------------
        public override void OnActionExecuting(ActionExecutingContext context)
        {//// overview:
         //// 判斷目前執行的動作是否為特定Action，如果會員沒登入，紀錄原本要使用的action至session(Key:RedirectAfterLogin)、
         //// txtProductId(Key:TempProductId)、 deliverytypeid(Key:TempDeliverytypeid)、 txtCount(Key:TempTxtCount)，最後跳轉至登入頁面
            base.OnActionExecuting(context); // 調用基類的 OnActionExecuting 方法來進行基本驗證
            if (context.ActionDescriptor.DisplayName.Contains("QuickAddToCart"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    int txtProductId = (int)context.ActionArguments["txtProductId"];
                    HttpContext.Session.SetInt32("TempProductId", txtProductId);
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName);
                    context.Result = new RedirectToActionResult("Login", "Home", null);
                }
            }
            else if (context.ActionDescriptor.DisplayName.Contains("CartView"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName);
                    context.Result = new RedirectToActionResult("Login", "Home", null);
                }
            }
            else if (context.ActionDescriptor.DisplayName.Contains("AddToCartAndReturnCarView"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    if (context.ActionArguments.ContainsKey("txtProductId"))
                    { HttpContext.Session.SetInt32("TempProductId", (int)context.ActionArguments["txtProductId"]); }
                    if (context.ActionArguments.ContainsKey("deliverytypeid"))
                    { HttpContext.Session.SetInt32("TempDeliverytypeid", (int)context.ActionArguments["deliverytypeid"]); }
                    if (context.ActionArguments.ContainsKey("txtCount"))
                    { HttpContext.Session.SetInt32("TempTxtCount", (int)context.ActionArguments["txtCount"]); }
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName);
                    context.Result = new RedirectToActionResult("Login", "Home", null);
                }
            }
            else if (context.ActionDescriptor.DisplayName.Contains("AddToCart"))
            {
                if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
                {
                    if (context.ActionArguments.ContainsKey("txtProductId"))
                    { HttpContext.Session.SetInt32("TempProductId", (int)context.ActionArguments["txtProductId"]); }
                    if (context.ActionArguments.ContainsKey("deliverytypeid"))
                    { HttpContext.Session.SetInt32("TempDeliverytypeid", (int)context.ActionArguments["deliverytypeid"]); }
                    if (context.ActionArguments.ContainsKey("txtCount"))
                    { HttpContext.Session.SetInt32("TempTxtCount", (int)context.ActionArguments["txtCount"]); }
                    HttpContext.Session.SetString("RedirectAfterLogin", context.ActionDescriptor.DisplayName);
                    context.Result = new JsonResult(new { needToLogin = true });
                }
            }
        }

        public IActionResult CartView()
        {// 顯示購物車畫面
            TMember loggedInUser = GetLoggedInUser();
            TMember userFromDatabase = _db.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
            ViewBag.userFromDatabase = userFromDatabase;
            int points = (int)(userFromDatabase?.Point ?? 0);
            ViewBag.MemberPoints = points;
            List<SSJ_CShoppingCarItem> cart = GetCartItems();
            return View(cart);
        }

        public IActionResult CartView_ConvenienceStore()
        {// 顯示購物車畫面
            TMember loggedInUser = GetLoggedInUser();
            TMember userFromDatabase = _db.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
            ViewBag.userFromDatabase = userFromDatabase;
            int points = (int)(userFromDatabase?.Point ?? 0);
            ViewBag.MemberPoints = points;
            List<SSJ_CShoppingCarItem> cart_ConvenienceStore = GetCartItems();
            return View(cart_ConvenienceStore);
        }


        public ActionResult AddToCart(SSJ_CShoppingCarItem vm, int txtProductId, int txtCount, int deliverytypeid)
        {
            if (txtCount <= 0)
            { return Json(new { success = false, message = "Invalid quantity." }); }
            if (deliverytypeid != 0)
            { HttpContext.Session.SetInt32("DeliveryType", deliverytypeid); }
            if (vm == null)
            { return Json(new { success = false, message = "No product details provided." }); }
            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == txtProductId);
            if (p == null)
            { return Json(new { success = false, message = "Product not found." }); }
            if (deliverytypeid == 1)
            { HandleCart(p, txtProductId, txtCount, deliverytypeid, GetCartItems, SaveCartItems); }
            else if (deliverytypeid == 2)
            { HandleCart(p, txtProductId, txtCount, deliverytypeid, GetCartItems_ConvenienceStore, SaveCartItems_ConvenienceStore); }
            else
            { return Json(new { success = false, message = "Invalid delivery type." }); }
            return Json(new { success = true, message = "Product added to cart." });
        }


        public ActionResult AddToCartAndReturnCarView(SSJ_CShoppingCarItem vm, int txtProductId, int txtCount, int deliverytypeid)
        {
            if (deliverytypeid != 0)
            {
                HttpContext.Session.SetInt32("DeliveryType", deliverytypeid);
            }

            if (vm == null)
            {
                return Json(new { success = false, message = "No product details provided." });
            }

            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == txtProductId);
            if (p == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            if (deliverytypeid == 1)
            {
                HandleCart(p, txtProductId, txtCount, deliverytypeid, GetCartItems, SaveCartItems);
                return RedirectToAction("CartView");
            }
            else if (deliverytypeid == 2) //2代表超商門市取貨
            {
                HandleCart(p, txtProductId, txtCount, deliverytypeid, GetCartItems_ConvenienceStore, SaveCartItems_ConvenienceStore);
                return RedirectToAction("CartView_ConvenienceStore");
            }
            else
            {
                return Json(new { success = false, message = "Invalid delivery type." });
            }
        }

        public ActionResult QuickAddToCart(int txtProductId)
        { // 購物頁商品快速加入商品到購物車
            // 設定預設的配送選項
            int defaultCount = 1;
            int defaultDeliveryOption = 1;
            int productId = txtProductId;
            // 用GetAndUpdateCart將商品加入購物車
            GetAndUpdateCart(productId, defaultCount, defaultDeliveryOption);
            return RedirectToAction("CartView");
        }

        [HttpPost]
        public ActionResult ConfirmPurchase(SSJ_ConfirmPurchaseViewModel vm, string SelectedProducts) /*int txtProductId, int txtCount, int deliverytypeid*/
        {
            //讀取購物車
            var cart = GetCartItems();
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("CartView");
            }
            else //test
            { Console.WriteLine(cart); }

            //將登入者存成loggedUserId
            var user = GetLoggedInUser();
            int loggedUserId = user.MemberId;

            // 將字符串反序列化為整數列表，Session中獲取CHKBox被選中的ID
            List<int> selectedProductIds = JsonSerializer.Deserialize<List<int>>(SelectedProducts);

            // 創建新的訂單並儲存
            int totalUsagePoints = 0;
            using (var db = _db)
            {
                TOrder order = new TOrder
                {
                    OrderDate = DateTime.Now,
                    BuyerId = loggedUserId,
                    OrderStatus = "3",//結帳後為3
                    OrderCatagory = 1,//買賣為1
                    ConvienenNum = "0",//超商尚未串接
                    DeliveryFee = GetDeliveryFee(vm.DeliveryType)//用函式尋找DeliveryFee
                    //DeliveryFee目前讀不到值都回0，應該做個加總
                };
                db.TOrders.Add(order);
                var selectedItems = cart.Where(item => selectedProductIds.Contains(item.ProductId));
                // 對於購物車中的每一項商品，都在TOrderDetail表中新增一個詳細資料行
                foreach (var item in selectedItems)
                {
                    TOrderDetail orderDetail = new TOrderDetail
                    {
                        OrderId = order.OrderId,
                        OrderDate = order.OrderDate,
                        DeliveryTypeId = item.DeliveryTypeID,
                        DeliveryAddress = vm.DeliveryAddress,
                        ProductId = item.ProductId,
                        ProductUnitPoint = item.point,
                        OrderQuantity = item.count,
                        SellerId = item.SellerId,
                    };
                    _db.TOrderDetails.Add(orderDetail);
                    totalUsagePoints += item.小計;
                }
                order.OrderTotalUsagePoints = totalUsagePoints;

                try
                {
                    _db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    LogDbUpdateException(e);  // A helper function to handle and log the exception
                }

                //更新會員點數
                TMember currentMember = db.TMembers.FirstOrDefault(t => t.MemberId == loggedUserId);
                if (currentMember != null)
                {
                    currentMember.Point -= totalUsagePoints;  // 減去已使用的點數
                    db.SaveChanges();  // 儲存變更至資料庫
                }

                // 購買後從Session中清除購物車
                cart.RemoveAll(item => selectedProductIds.Contains(item.ProductId));
                string json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(SSJ_CDictionary.SK_PURCAHSED_PRODUCTS_LIST, json);
            }
            return RedirectToAction("CartView");
        }

        [HttpPost]
        public JsonResult UpdateCartItem(int productId, int? newCount, int? deliveryTypeId)
        {//更新小計、totalPoint用
            // 從Session中獲取購物車列表
            var cart = GetCartItems();
            // 在購物車中尋找與指定ID匹配的商品
            var itemToUpdate = cart.FirstOrDefault(item => item.ProductId == productId);
            // 如果找不到該商品，返回失敗
            if (itemToUpdate == null) return Json(new { success = false, message = "Product not found" });
            // 如果有值更新商品數量，沒有不更新
            if (newCount.HasValue)
            { itemToUpdate.count = newCount.Value; }
            // 如果有值更新商品deliveryTypeId，沒有不更新
            if (deliveryTypeId.HasValue)
            { itemToUpdate.DeliveryTypeID = deliveryTypeId.Value; }
            // 保存新的購物車列表到Session
            SaveCartItems(cart);
            // 返回成功和更新後的資訊
            return Json(new { success = true, subtotalPoints = itemToUpdate.小計, itemToUpdate.count, totalPoints = cart.Sum(item => item.小計) });
        }
        [HttpPost]
        public JsonResult DeleteCartItem(int productId)
        {// 從購物車Session中刪除某一商品
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
        //        當你的購物平台允許買家從多個賣家那裡購買商品時，"訂單拆分" 是一個常見而且有助於管理的策略。

        //訂單拆分的詳細說明：
        //基本概念：
        //訂單拆分是指當一個買家在結帳時，他的購物車中的商品來自多個賣家，系統會為每個賣家生成一個獨立的訂單，而不是將所有商品放在一個訂單中。

        //操作流程：

        //買家選擇商品加入購物車。
        //結帳時，系統檢查購物車中的商品，確定有多少個賣家。
        //對於購物車中的每個賣家，系統生成一個新的訂單編號。
        //買家結帳完成後，將會看到多個訂單確認信息，每個賣家一個訂單。
        //舉例：
        //假設買家小王的購物車中有以下商品：

        //商品A，賣家：張三
        //商品B，賣家：張三
        //商品C，賣家：李四
        //當小王進行結帳時，系統會生成兩個訂單：

        //訂單#001：
        //商品A
        //商品B
        //賣家：張三
        //訂單#002：
        //商品C
        //賣家：李四
        //好處：
        //賣家管理：張三和李四都會收到與他們商品相關的訂單通知。張三不需要關心商品C，李四也不需要關心商品A和商品B。這減少了混淆和管理上的困難。

        //出貨與配送：由於每個訂單都是獨立的，所以每個賣家都可以獨立處理配送。如果所有商品都在同一個訂單中，則可能需要集中配送，這會增加復雜性。

        //追踪與管理：對於買家來說，他們可以單獨追踪每個賣家的訂單。例如，如果商品C配送延遲，小王可以只關心訂單#002，而不需要擔心整個購物車的訂單。

        //退貨與售後：在售後服務方面，如果小王想退貨商品C，他可以直接與李四聯繫，而不影響訂單#001。

        //這種設計策略已經在很多大型的電商平台上得到了應用，尤其是在多供應商或多賣家的平台中。
    }
}
