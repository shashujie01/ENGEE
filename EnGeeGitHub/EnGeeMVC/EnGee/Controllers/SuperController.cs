using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjMvcCoreDemo.Models;

namespace EnGee.Controllers
{
    public class SuperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "LoginLayout"
                }));
            }
        }
        //public override void OnActionExecuting(ActionExecutingContext context)//SSJ沒寫完
        //{//登入後重新導向到指定位置
        //    base.OnActionExecuting(context);
        //    if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
        //    {
        //        HttpContext.Session.SetString("RedirectAction", context.ActionDescriptor.RouteValues["action"]);
        //        HttpContext.Session.SetString("RedirectController", context.ActionDescriptor.RouteValues["controller"]);

        //        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
        //        {
        //            controller = "Home",
        //            action = "LoginLayout"
        //        }));
        //    }
        //}
        //protected bool IsUserLoggedIn()
        //    //SSJ
        //    //檢查是否為登入狀態
        //{
        //    return HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER);
        //}
    }
}
