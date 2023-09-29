using EnGee.Data;
using EnGee.Models;
using EnGee.Repositories;
using EnGee.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjMvcCoreDemo.Models;
using System;
using System.Security.Claims;
using System.Text.Json;



namespace EnGee.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly MemberFavoriteRepository _favoriteRepository;
        private readonly EngeeContext _dbContext;  // DbContext 名稱。

        public FavoriteController(EngeeContext dbContext, MemberFavoriteRepository favoriteRepository)
        {
            _dbContext = dbContext;
            _favoriteRepository = favoriteRepository;

        }

        [HttpGet]
        public async Task<IActionResult> ManageFavorites()
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (string.IsNullOrEmpty(userJson))
            {
                // 請先登錄
                return RedirectToAction("Login", "Home"); //請修改為正確的登入控制器名稱
            }

            TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
            var memberId = loggedInUser.MemberId;

            var favorites = await _dbContext.TMemberFavorites
                                           .Include(f => f.Product)
                                           .Where(f => f.MemberId == memberId)
                                           .ToListAsync();

            return View(favorites);
        }
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int productId, bool addFavoriteType)
        {
            try
            {
                // 檢查用戶是否已登錄
                string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                if (string.IsNullOrEmpty(userJson))
                {
                    return Json(new { success = false, message = "請先登錄", redirectToLogin = true });
                }

                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
                var memberId = loggedInUser.MemberId;

                // 使用存儲庫來檢查收藏夾中是否已存在該產品
                var existingFavorite = _favoriteRepository.IsProductFavoriteForUser(memberId, productId);
                if (existingFavorite)
                {
                    return Json(new { success = false,message = "產品已在收藏夾中!" });
                }

                var favorite = new TMemberFavorite
                {
                    ProductId = productId,
                    MemberId = memberId,
                    AddFavoriteDate = DateTime.Now,
                    AddFavoriteType = addFavoriteType
                };

                // TODO: 你也可以在存儲庫裡加入方法來處理加入收藏夾的操作
                // _favoriteRepository.AddToFavorites(favorite);
                _dbContext.TMemberFavorites.Add(favorite);
                await _dbContext.SaveChangesAsync();

                return Json(new { success = true, message = "已經成功加入我的最愛囉" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int favoriteId)
        {
            try
            {
                string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                if (string.IsNullOrEmpty(userJson))
                {
                    return RedirectToAction("Login", "Account"); //请根据您的项目中的登录控制器和动作来修改
                }

                TMember loggedInUser = JsonSerializer.Deserialize<TMember>(userJson);
                var memberId = loggedInUser.MemberId;

                var favorite = await _dbContext.TMemberFavorites.FindAsync(favoriteId);
                if (favorite == null || favorite.MemberId != memberId)
                {
                    TempData["ErrorMessage"] = "无法找到或删除其他用户的收藏"; // 使用 TempData 传递错误消息
                    return RedirectToAction("ManageFavorites");
                }

                _dbContext.TMemberFavorites.Remove(favorite);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "已成功从我的最爱中删除"; // 使用 TempData 传递成功消息
                return RedirectToAction("ManageFavorites");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; // 使用 TempData 传递异常消息
                return RedirectToAction("ManageFavorites");
            }
        }


    }
}
