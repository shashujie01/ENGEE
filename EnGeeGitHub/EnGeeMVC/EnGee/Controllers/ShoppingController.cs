using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EnGee.Models;
using EnGee.ViewModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using prjMvcCoreDemo.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using Microsoft.Extensions.Hosting.Internal;


namespace EnGee.Controllers
{
    public class ShoppingController : SuperController
    {//TODO特定時間清除多餘SESSION
        private readonly EngeeContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ShoppingController(EngeeContext db, CHI_CUserViewModel userViewModel, IWebHostEnvironment hostingEnvironment) : base(userViewModel)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

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
//--------------------------------------
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

            string jsonPath = Path.Combine(_hostingEnvironment.WebRootPath, "lib", "stores", "711Stores.json");
            var jsonData = System.IO.File.ReadAllText(jsonPath);
            var stores = JsonSerializer.Deserialize<List<SSJ_CShoppingCarItem>>(jsonData);

            ViewBag.Stores = stores;
            List<SSJ_CShoppingCarItem> cart = GetCartItems();
            return View(cart);
        }

        public ActionResult AddToCart(SSJ_CShoppingCarItem vm, int txtProductId, int txtCount, int deliverytypeid)
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
            List<SSJ_CShoppingCarItem> cart = GetCartItems();
            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == txtProductId);
            if (existingItem != null)
            {
                existingItem.count += txtCount;
            }
            else
            {
                cart.Add(new SSJ_CShoppingCarItem
                {
                    tproduct = p,
                    SellerId = p.SellerId,
                    point = (int)p.ProductUnitPoint,
                    ProductId = txtProductId,
                    count = txtCount,
                    ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                    DeliveryTypeID = deliverytypeid
                });
            }
            SaveCartItems(cart);
            return Json(new { success = true, message = "Product added to cart." });
        }

        public ActionResult AddToCartAndReturnCarView(SSJ_CShoppingCarItem vm, int txtProductId, int txtCount, int deliverytypeid)
        {//Detail中的直接購買(跳轉至cartview)
            if (deliverytypeid != 0)
            {
                HttpContext.Session.SetInt32("DeliveryType", deliverytypeid);
            }

            if (vm == null)
            {
                return Json(new { success = false, message = "No product details provided." });
            }
            else//test
            {
                Console.WriteLine(vm.ProductId);
                Console.WriteLine(vm.count);
                Console.WriteLine(vm.DeliveryTypeID);
                Console.WriteLine(vm.DeliveryType);
                Console.WriteLine(vm.小計);
            }

            TProduct p = _db.TProducts.FirstOrDefault(t => t.ProductId == txtProductId);
            if (p == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            List<SSJ_CShoppingCarItem> cart = GetCartItems();
            SSJ_CShoppingCarItem existingItem = cart.FirstOrDefault(i => i.ProductId == txtProductId);

            if (existingItem != null)
            {//檢查是否購物車內已經存在
                existingItem.count += (int)txtCount;
            }
            else
            {
                cart.Add(new SSJ_CShoppingCarItem
                {
                    tproduct = p,
                    SellerId = p.SellerId,
                    point = (int)p.ProductUnitPoint,
                    ProductId = txtProductId,
                    count = txtCount,
                    ProductImagePath = $"/images/ProductImages/{p.ProductImagePath}",
                    DeliveryTypeID = (int)deliverytypeid
                });
            }
            SaveCartItems(cart);
            return RedirectToAction("CartView");
        }

        public ActionResult QuickAddToCart(int txtProductId)
        { // 購物頁商品快速加入商品到購物車
            // 設定預設的配送選項
            int defaultCount = 1;
            int defaultDeliveryOption;
            var product= _db.TProducts.FirstOrDefault(p=>p.ProductId==txtProductId);
            if (product.DeliveryTypeId == 2)
            {
                defaultDeliveryOption = 2;
            }
            else 
            {
                defaultDeliveryOption = 1;
            }
            int productId = txtProductId;
            // 用GetAndUpdateCart將商品加入購物車
            GetAndUpdateCart(productId, defaultCount, defaultDeliveryOption);
            return RedirectToAction("CartView");
        }

        [HttpPost]
        public ActionResult ConfirmPurchase(SSJ_ConfirmPurchaseViewModel vm, string SelectedProducts)
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
            List<int> selectedProductIds = JsonSerializer.Deserialize<List<int>>(SelectedProducts);
            var selectedItems = cart.Where(item => selectedProductIds.Contains(item.ProductId));
            List<int> selectedDeliveryTypeIds = selectedItems.Select(item => item.DeliveryTypeID).Distinct().ToList();
            List<TDeliveryType> selectedDeliveryTypes = _db.TDeliveryTypes.Where(d => selectedDeliveryTypeIds.Contains(d.DeliveryTypeId)).ToList();
            int deliveryFee = CalculateTotalDeliveryFee(selectedDeliveryTypes);

            // 創建新的訂單並儲存
            int totalUsagePoints = 0;
            {
                TOrder order = new TOrder
                {
                    OrderDate = DateTime.Now,
                    BuyerId = loggedUserId,
                    OrderStatus = "3",//結帳後為3
                    OrderCatagory = 1,//買賣為1
                    ConvienenNum = "0",//超商用另外邏輯處理->因為要結訓先不刪除
                    DeliveryFee = deliveryFee,//用函式尋找DeliveryFee
                    //ReceiverName=vm.ReceiverName,
                    //ReceiverTEL = vm.ReceiverTEL
    };

                // 計算 totalUsagePoints
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
                    totalUsagePoints += item.小計;
                }
                TMember currentMember = _db.TMembers.FirstOrDefault(t => t.MemberId == loggedUserId);

                if (currentMember.Point < totalUsagePoints + deliveryFee)
                {
                    TempData["ErrorMessage"] = "您的點數不足"; 
                    return RedirectToAction("CartView", "Shopping");
                }
                //-----------
                // 如果點數足夠，設置order.OrderTotalUsagePoints，並將訂單和訂單詳細信息儲存到資料庫
                using (var db = _db)
                { 
                db.TOrders.Add(order);
                db.SaveChanges();
                
                // 對於購物車中的每一項商品，都在TOrderDetail表中新增一個詳細資料行
                foreach (var item in selectedItems)
                {
                        if (item.DeliveryTypeID == 1)
                        {
                            item.DeliveryAddress_homeDelivery = vm.DeliveryAddress_homeDelivery;
                        }
                        else if (item.DeliveryTypeID == 2)
                        {
                            item.DeliveryAddress_storePickup = vm.DeliveryAddress_storePickup;
                        }
                        TOrderDetail orderDetail = new TOrderDetail
                    {
                        OrderId = order.OrderId,
                        OrderDate = order.OrderDate,
                        DeliveryTypeId = item.DeliveryTypeID,
                        DeliveryAddress = item.DeliveryAddress,
                        ProductId = item.ProductId,
                        ProductUnitPoint = item.point,
                        OrderQuantity = item.count,
                        SellerId = item.SellerId,
                    };
                    _db.TOrderDetails.Add(orderDetail);
                }
                order.OrderTotalUsagePoints = totalUsagePoints+ deliveryFee;

                try
                {
                    _db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    LogDbUpdateException(e);  // A helper function to handle and log the exception
                }

                //更新會員點數
                 currentMember = db.TMembers.FirstOrDefault(t => t.MemberId == loggedUserId);
                if (currentMember != null)
                {
                    currentMember.Point = currentMember.Point -totalUsagePoints- deliveryFee;  // 減去已使用的點數
                    db.SaveChanges();  // 儲存變更至資料庫
                }

                // 減少商品庫存
                foreach (var item in selectedItems)
                {
                    var product = db.TProducts.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product != null)
                    {
                        product.ProductRemainingQuantity -= item.count;

                        // Check if ProductRemainingQuantity is 0
                        if (product.ProductRemainingQuantity <= 0)
                        {
                            product.ProductSaleStatus = 1;
                            product.ProductRemainingQuantity = 0; // 確保數量不會低於0
                        }
                    }
                }
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

        [HttpGet]
        public async Task<IActionResult> GetProductSellerID(int value)
        {
            var seller = await _db.TMembers.FirstOrDefaultAsync(s => s.MemberId == value);

            if (seller != null)
            {
                return Json(seller.Username);
            }
            else
            {
                return Json("查無賣家資料");
            }
        }
        private int CalculateTotalDeliveryFee(List<TDeliveryType> selectedDeliveryTypeIDs)
        {
            var totalFee = 0;
            var processedDeliveryTypeIds = new HashSet<int>();

            foreach (var item in selectedDeliveryTypeIDs)
            {
                if (processedDeliveryTypeIds.Contains(item.DeliveryTypeId))
                {
                    continue;
                }

                var deliveryFee = _db.TDeliveryTypes
                                    .Where(d => d.DeliveryTypeId == item.DeliveryTypeId)
                                    .Select(d => d.DeliveryFee)
                                    .FirstOrDefault();

                if (deliveryFee.HasValue)
                    totalFee += deliveryFee.Value;

                processedDeliveryTypeIds.Add(item.DeliveryTypeId);
            }

            return totalFee;
        }
        [HttpPost]
        public IActionResult GetTotalDeliveryFee(List<TDeliveryType> selectedDeliveryTypeIDs)
        {
            var totalFee = 0;
            var processedDeliveryTypeIds = new HashSet<int>();  // 用來記錄已經處理過的deliveryTypeId

            foreach (var item in selectedDeliveryTypeIDs)
            {
                if (processedDeliveryTypeIds.Contains(item.DeliveryTypeId))
                {
                    // 如果這個deliveryTypeId已經被處理過，則跳過
                    continue;
                }

                var deliveryFee = _db.TDeliveryTypes
                                    .Where(d => d.DeliveryTypeId == item.DeliveryTypeId)
                                    .Select(d => d.DeliveryFee)
                                    .FirstOrDefault();

                if (deliveryFee.HasValue)
                    totalFee += deliveryFee.Value;

                processedDeliveryTypeIds.Add(item.DeliveryTypeId);  // 加入到已處理集合中
            }

            return Json(new { totalFee = totalFee });
        }
    }
}
