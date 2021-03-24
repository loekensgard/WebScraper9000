using System.Collections.Generic;
using System.Threading.Tasks;
using WebScraper9000.Models;

namespace WebScraper9000.Interfaces
{
    public interface IDiscordService
    {
        Task SendDiscordMessage(List<InStockItem> list);
        Task SendError(string channel, string error);
        Task SendNoItems(string channel, string channelid);
    }
}
