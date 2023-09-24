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
    public class SSJ_ShoppingListController : Controller
    {
        private readonly EngeeContext _db;

        public SSJ_ShoppingListController(EngeeContext db)
        {
            _db = db;
        }
        public IActionResult ShoppingList_admin(int page = 1)
        {
            int pageSize = 12; // 每頁顯示的數量

            // 取得tOrders和tOrderDetail的資料，並連接他們
            var orderDetailsQuery = from order in _db.TOrders
                                    join orderDetail in _db.TOrderDetails on order.OrderId equals orderDetail.OrderId
                                    select new ShoppingListViewModel
                                    {
                                        OrderID = order.OrderId,
                                        OrderDate = order.OrderDate,
                                        OrderTotalUsagePoints = order.OrderTotalUsagePoints,
                                        BuyerID = order.BuyerId,
                                        OrderStatus = order.OrderStatus,
                                        OrderCatagory = order.OrderCatagory,
                                        ConvienenNum = order.ConvienenNum,
                                        DeliveryFee = order.DeliveryFee,
                                        OrderDetailID = orderDetail.OrderDetailId,
                                        ProductID = orderDetail.ProductId,
                                        ProductUnitPoint = orderDetail.ProductUnitPoint,
                                        OrderQuantity = orderDetail.OrderQuantity,
                                        SellerID = orderDetail.SellerId,
                                        DeliveryTypeID = orderDetail.DeliveryTypeId,
                                        DeliveryAddress = orderDetail.DeliveryAddress
                                    };

            var totalCount = orderDetailsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var ordersList = orderDetailsQuery.Skip((page - 1) * pageSize)
                                               .Take(pageSize)
                                               .ToList();

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View(ordersList);
        }
    }
}
