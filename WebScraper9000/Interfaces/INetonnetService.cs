using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebScraper9000.Models;

namespace WebScraper9000.Interfaces
{
    public interface INetonnetService
    {
        Task<List<InStockItem>> GetItemInStockFromNetonnet(string url, string name, string discordChannel, string channelId);
    }
}
