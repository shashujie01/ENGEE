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
        {
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
    }
}
