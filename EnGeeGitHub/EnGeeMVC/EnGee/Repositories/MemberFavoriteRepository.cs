using EnGee.Models;
using EnGee.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EnGee.Repositories
{
    public class MemberFavoriteRepository
    {
        private readonly EngeeContext dbContext;

        public MemberFavoriteRepository(EngeeContext context)
        {
            dbContext = context;
        }

        public bool IsProductFavoriteForUser(int memberId, int productId)
        {
           
            return dbContext.TMemberFavorites.Any(f => f.Member.MemberId == memberId && f.Product.ProductId == productId && f.AddFavoriteType == true); //Favoritetype=true=1=壹點贈送
        }
    }
}

//要去Program.cs .添加 MemberFavoriteRepository 到依賴注入容器
//builder.Services.AddScoped<MemberFavoriteRepository>();