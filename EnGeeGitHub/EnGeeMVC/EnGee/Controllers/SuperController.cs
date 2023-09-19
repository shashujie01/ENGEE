using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjMvcCoreDemo.Models;
using System.Text.Json;



namespace EnGee.Controllers
{
    public class SuperController : Controller
    {
        private readonly CHI_CUserViewModel _userViewModel;

        public SuperController(CHI_CUserViewModel userViewModel)
        {
            _userViewModel = userViewModel;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        { // 使用ActionExecutingContext:有繼承SuperController的Controller中的每個Action執行前會被呼叫
          // 調用基類的OnActionExecuting方法
            base.OnActionExecuting(context);
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);

            if (string.IsNullOrEmpty(userJson))
            {
                // 沒登入
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login"
                }));
            }
            else
            {
                // 有登入,傳到共用的viewmodel
                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
                _userViewModel.LoggedInUser = loggedInUser;
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
        //            action = "Login"
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
