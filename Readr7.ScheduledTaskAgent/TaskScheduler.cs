using Microsoft.Phone.Scheduler;
using Readr7.Model;
using Microsoft.Phone.Shell;
using System.Linq;

namespace Readr7.ScheduledTaskAgent
{
    public class ScheduledAgent : Microsoft.Phone.Scheduler.ScheduledTaskAgent
    {
        protected override void OnInvoke(ScheduledTask task)
        {
            var service = new GoogleReaderService();

            if (service.IsAuthenticated)
            {
                service.GetUnreadCount(count =>
                {
                    ShellTile.ActiveTiles.First().Update(new StandardTileData()
                    {
                        Count = count
                    });
                    NotifyComplete();
                });
            }
            else
            {
                NotifyComplete();
            }
        }
    }
}
