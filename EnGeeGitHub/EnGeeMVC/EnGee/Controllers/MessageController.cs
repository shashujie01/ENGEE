using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using prjMvcCoreDemo.Models;

namespace EnGee.Controllers
{
    public class MessageController : Controller
    {
        private readonly EngeeContext _dbContext;  // 根據您的 DbContext 名稱，這可能需要修改。

        public MessageController(EngeeContext dbContext)  // 使用 DI（依賴注入）將 DbContext 注入控制器。
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddMessage(string MessageArea, string MessageContent, string userJson, int ProductId)
        {
            try
            {
                TMember loginerUser;

                var userSessionJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                if (string.IsNullOrEmpty(userSessionJson))
                {
                    // 如果用戶未登錄，則設置一個默認的用戶。這裡需要你根據實際需求進行調整。
                    loginerUser = new TMember { MemberId = 184 };  // 指定一個默認的 MemberId，可能需要根據實際情況調整。
                }
                else
                {
                    loginerUser = JsonConvert.DeserializeObject<TMember>(userSessionJson);
                }

                TMessage message = new TMessage
                {
                    MemberId = loginerUser.MemberId,
                    MessageArea = MessageArea,
                    MessageContent = MessageContent,
                    ThumpUp = 0,
                    ProductId = ProductId,
                    MessageDate = DateTime.Now
                };

                _dbContext.TMessages.Add(message);
                _dbContext.SaveChanges();

                return RedirectToAction("DetailsJING", "JING_Product", new { id = ProductId });
            }
            catch (Exception ex)

            {
            
                // 這裡可以記錄異常和/或將其顯示給用戶
                Console.WriteLine(ex.Message);
                // 返回一個包含錯誤訊息的視圖或其他適當的回應
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }

            [HttpPost]
        public IActionResult DeleteMessage(int messageId)
        {
            var message = _dbContext.TMessages.Find(messageId);
            if (message == null)
            {
                return NotFound(); // or redirect to an error page
            }

            int productId = message.ProductId;

            _dbContext.TMessages.Remove(message);
            var changesSaved = _dbContext.SaveChanges();

            if (changesSaved == 0)
            {
                // Log this or handle it appropriately.
                // This means the message was not deleted.
                // For now, let's redirect to an error page or home page.
                return RedirectToAction("DetailsJING", new { id = productId });
            }

            return RedirectToAction("DetailsJING", "JING_Product", new { id = productId });
        }

    

        //[HttpPost]
        //public ActionResult AddMessage(string MessageArea, string MessageContent, string userJson, int ProductId)
        //{
        //    var userSessionJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
        //    if (string.IsNullOrEmpty(userSessionJson))
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    // 使用 Newtonsoft.Json 來反序列化
        //    TMember loginerUser = JsonConvert.DeserializeObject<TMember>(userSessionJson);

        //    TMessage message = new TMessage
        //    {
        //        MemberId = loginerUser.MemberId,
        //        MessageArea = MessageArea,
        //        MessageContent = MessageContent,
        //        ThumpUp = 0,
        //        ProductId = ProductId
        //    };

        //    _dbContext.TMessages.Add(message);
        //    _dbContext.SaveChanges();

        //    // 依據不同的留言區域重定向到不同的页面
        //    switch (MessageArea)
        //    {
        //        //case "許願池": return RedirectToAction("DetailsJING");
        //        case "一點贈送": return RedirectToAction("DetailsJING", "JING_Product", new { id = ProductId });
        //        //case "買賣專區": return RedirectToAction("TradeZonePage");
        //        default: return RedirectToAction("Index", "Home");
        //    }
        //}




    }
}
