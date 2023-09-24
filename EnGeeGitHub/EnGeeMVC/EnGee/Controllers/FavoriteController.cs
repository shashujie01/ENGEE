using EnGee.Data;
using EnGee.Models;
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

        private readonly EngeeContext _dbContext;  // DbContext 名稱。

        public FavoriteController(EngeeContext dbContext)  // 使用 DI（依賴注入）將 DbContext 注入控制器。
        {
            _dbContext = dbContext;
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

                var existingFavorite = await _dbContext.TMemberFavorites
                                   .AnyAsync(f => f.ProductId == productId && f.MemberId == memberId);
                if (existingFavorite)
                {
                    return Json(new { success = false, message = "產品已在收藏夾中!" });
                }

                var favorite = new TMemberFavorite
                {
                    ProductId = productId,
                    MemberId = memberId,
                    AddFavoriteDate = DateTime.Now,
                    AddFavoriteType = addFavoriteType
                };

                _dbContext.TMemberFavorites.Add(favorite);
                await _dbContext.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}