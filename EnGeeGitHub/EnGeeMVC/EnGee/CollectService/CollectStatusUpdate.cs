using EnGee.Data;
using EnGee.Models;

namespace EnGee.CollectService
{
    public class CollectStatusUpdate : IHostedService, IDisposable
    {
        
        private readonly IServiceScopeFactory _scopeFactory; // 讀取資料庫內容
        private Timer _timer;

        public CollectStatusUpdate(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow; //   UTC不受時間限制
            var scheduledTime = new DateTime(now.Year, now.Month, now.Day, 00,0,0, DateTimeKind.Utc); //    每日凌晨12點

            if (now > scheduledTime) 
            {
                scheduledTime = scheduledTime.AddDays(1);  //   設定每日同一時間執行
            }

            var delayTime = scheduledTime - now;  //  計算距離下次執行時間還有多久

            _timer = new Timer(DoWork, null, delayTime, TimeSpan.FromDays(1));  // 每天同一時間執行
            return Task.CompletedTask;
        }

        public void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EngeeContext>();
                var now = DateTime.Now.Date;
                var expiredCollects = dbContext.TCollects
                    .Where(c => (c.CollectStatus != false && c.CollectEndDate < now) || (c.CollectStatus != false && c.CollectAmount == 0))
                    .ToList();
                foreach (var collect in expiredCollects)
                {
                    collect.CollectStatus = false;
                }
                dbContext.SaveChanges();
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
