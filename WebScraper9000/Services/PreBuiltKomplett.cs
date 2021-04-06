using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebScraper9000.Configurations;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class PreBuiltKomplett : IStoreService
    {
        public async Task<IEnumerable<InStockItem>> GetItemInStock(ItemsIWant item)
        {
            var list = new List<InStockItem>();

            if (string.IsNullOrEmpty(item.KomplettUrl))
            {
                return list;
            }

            if (!item.Name.Contains("Komplett a240 Epic Gaming PC"))
            {
                return list;
            }

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(item.KomplettUrl, Encoding.UTF8, CancellationToken.None);

            var products = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' buy-button-section ')]");

            foreach (var product in products)
            {
                var inStock = product.SelectSingleNode(".//div[contains(@class, 'buy-button')]");

                if (inStock != null && inStock.InnerText.Contains("Legg i handlevogn"))
                {
                    list.Add(new InStockItem { Url = item.KomplettUrl, Name = item.Name, Count = 0, Channel = item.DiscordChannel, Store = "Komplett.no", ChannelId = item.DiscordChannelId });
                }
            }

            return list;
        }
    }
}
