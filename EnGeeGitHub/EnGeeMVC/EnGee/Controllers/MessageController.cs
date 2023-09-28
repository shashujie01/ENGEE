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

            var userSessionJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (string.IsNullOrEmpty(userSessionJson))
            {
                // 用户未登录，重定向到登录页面或显示错误
                return RedirectToAction("Login", "Home"); // 根据您的路由进行调整
            }

            var loginUser = JsonConvert.DeserializeObject<TMember>(userSessionJson);

            if (loginUser.MemberId != message.MemberId)
            {
                // 登录用户不是消息的作者，显示错误或重定向
                return View("Error", new ErrorViewModel { Message = "您没有权限删除这条消息" });
            }

            // 现在可以安全删除消息
            int productId = message.ProductId;

            _dbContext.TMessages.Remove(message);
            var changesSaved = _dbContext.SaveChanges();

            if (changesSaved == 0)
            {
                // 记录或合适处理
                // 这意味着消息未被删除
                // 现在，重定向到错误页面或主页面
                return RedirectToAction("DetailsJING", new { id = productId });
            }

            return RedirectToAction("DetailsJING", "JING_Product", new { id = productId });
        }

    }

}

    

      



