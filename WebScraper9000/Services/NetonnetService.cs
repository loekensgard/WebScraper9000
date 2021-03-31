using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebScraper9000.Configurations;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class NetonnetService : IStoreService
    {
        public async Task<IEnumerable<InStockItem>> GetItemInStock(ItemsIWant item)
        {
            var list = new List<InStockItem>();

            if (string.IsNullOrEmpty(item.NetonnetUrl))
            {
                return list;
            }

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(item.NetonnetUrl, Encoding.UTF8, CancellationToken.None);

            var products = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' panel panel-default clearfix ')]");

            foreach (var product in products)
            {
                var inStock = product.SelectSingleNode(".//span[contains(@class, 'stockStatusInStock')]");

                if (inStock != null)
                {

                    var productLink = product.SelectSingleNode(".//a");
                    if (productLink != null)
                    {
                        var hrefValue = productLink.GetAttributeValue("href", string.Empty);
                        list.Add(new InStockItem { Url = "https://netonnet.no" + hrefValue, Name = item.Name, Count = 0, Channel = item.DiscordChannel, Store = "NetOnNet.no", ChannelId = item.DiscordChannelId });
                    }
                }
            }

            return list;
        }
    }
}
