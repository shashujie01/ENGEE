using EnGee.Data;
using EnGee.Models;

namespace EnGee.CollectService
{
    public class CollectStatusUpdate : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer = null;

        public CollectStatusUpdate(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24)); 
            return Task.CompletedTask;
        }

        public void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EngeeContext>(); 
                var expiredCollects = dbContext.TCollects
                    .Where(c => (c.CollectStatus != false && c.CollectEndDate < DateTime.Now) || (c.CollectStatus != false && c.CollectAmount == 0))
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
