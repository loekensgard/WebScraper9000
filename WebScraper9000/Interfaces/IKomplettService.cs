using System.Collections.Generic;
using System.Threading.Tasks;
using WebScraper9000.Models;

namespace WebScraper9000.Interfaces
{
    public interface IKomplettService
    {
        Task<List<InStockItem>> GetItemInStockFromKomplett(string url, string name, string discordChannel);
    }
}
