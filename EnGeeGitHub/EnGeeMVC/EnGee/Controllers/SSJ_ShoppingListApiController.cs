using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;


namespace EnGee.Controllers
{//訂單使用API沒做完
    [Route("api/[controller]")]
    [ApiController]
    public class SSJ_ShoppingListApiController : ControllerBase
    {
        private readonly EngeeContext _db;

        public SSJ_ShoppingListApiController(EngeeContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = (from o in _db.TOrders
                         join m in _db.TMembers on o.BuyerId equals m.MemberId
                         where o.OrderId == id
                         select new SSJ_ShoppingListOrderViewModel
                         {
                             OrderID = o.OrderId,
                             OrderDate = o.OrderDate,
                             OrderTotalUsagePoints = o.OrderTotalUsagePoints,
                             BuyerID = o.BuyerId,
                             BuyerUsername = m.Username,
                             OrderStatus = o.OrderStatus,
                             OrderCatagory = o.OrderCatagory,
                             ConvienenNum = o.ConvienenNum,
                             DeliveryFee = o.DeliveryFee
                         }).FirstOrDefault();

            var orderDetails = (from orderDetail in _db.TOrderDetails
                                join product in _db.TProducts on orderDetail.ProductId equals product.ProductId
                                join member in _db.TMembers on orderDetail.SellerId equals member.MemberId
                                join deliveryType in _db.TDeliveryTypes on orderDetail.DeliveryTypeId equals deliveryType.DeliveryTypeId
                                where orderDetail.OrderId == id
                                select new SSJ_ShoppingListOrderDetailViewModel
                                {
                                    OrderID = orderDetail.OrderId,
                                     OrderDetailID = orderDetail.OrderDetailId,
                                     ProductID = orderDetail.ProductId,
                                     ProductName = product.ProductName,
                                     ProductImagePath = $"/images/ProductImages/{product.ProductImagePath}",
                                     ProductUnitPoint = orderDetail.ProductUnitPoint,
                                     OrderQuantity = orderDetail.OrderQuantity,
                                     SellerID = orderDetail.SellerId,
                                     SellerUsername = member.Username,
                                     DeliveryTypeID = orderDetail.DeliveryTypeId,
                                     DeliveryType = deliveryType.DeliveryType,
                                     DeliveryAddress = orderDetail.DeliveryAddress
                                }).ToList();

            if (order == null)
            {
                return NotFound();
            }

            var result = new SSJ_ShoppingListCombinedViewModel
            {
                Orders = new List<SSJ_ShoppingListOrderViewModel> { order },
                OrderDetails = orderDetails
            };

            return Ok(result);

        }

        [HttpPost]
        public IActionResult UpdateOrder([FromBody] SSJ_ShoppingListCombinedViewModel model)
        {
            if (model == null || model.Orders == null || model.OrderDetails == null)
            {
                return BadRequest("Invalid data.");
            }

            // ... your update logic here ...

            return Ok(new { Message = "Order updated successfully." });
        }
    }
}

