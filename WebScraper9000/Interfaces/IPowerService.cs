using System.Collections.Generic;
using System.Threading.Tasks;
using WebScraper9000.Models;

namespace WebScraper9000.Interfaces
{
    public interface IPowerService
    {
        Task<List<InStockItem>> GetItemInStockFromPower(string url, string name, string discordChannel);
    }
}
