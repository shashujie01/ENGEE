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
    }
}
