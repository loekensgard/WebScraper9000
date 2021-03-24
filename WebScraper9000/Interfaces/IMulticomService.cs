using System.Collections.Generic;
using System.Threading.Tasks;
using WebScraper9000.Models;

namespace WebScraper9000.Interfaces
{
    public interface IMulticomService
    {
        Task<List<InStockItem>> GetItemInStockFromMulticom(string url, string name, string discordChannel, string discordChannelId);
    }
}
