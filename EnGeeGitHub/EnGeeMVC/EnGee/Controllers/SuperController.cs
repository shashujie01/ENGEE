using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjMvcCoreDemo.Models;

namespace EnGee.Controllers
{
    public class SuperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        { // 使用ActionExecutingContext:有繼承SuperController的Controller中的每個Action執行前會被呼叫
          // 調用基類的OnActionExecuting方法
            base.OnActionExecuting(context);
            // 檢查Session是否包含已登入用戶的鍵值(SK_LOINGED_USER)
            // 如果不包含，則重定向到"Home"控制器的"LoginLayout"動作
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "LoginLayout"
                }));
            }
        }
    }
}
