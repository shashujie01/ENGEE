using EnGee.Models;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EnGee.Controllers
{
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
            var order = _db.TOrders.Where(o => o.OrderId == id)
                        .Select(o => new SSJ_ShoppingListOrderViewModel { /*... map properties ...*/ })
                        .FirstOrDefault();

            var orderDetails = _db.TOrderDetails.Where(od => od.OrderId == id)
                        .Select(od => new SSJ_ShoppingListOrderDetailViewModel { /*... map properties ...*/ })
                        .ToList();

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

