using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProshopService : IStoreService
    {
        public async Task<IEnumerable<InStockItem>> GetItemInStock(ItemsIWant item)
        {
            var list = new List<InStockItem>();

            if (string.IsNullOrEmpty(item.ProshopUrl))
            {
                return list;
            }

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(item.ProshopUrl, Encoding.UTF8, CancellationToken.None);

            var products = doc.DocumentNode.SelectNodes("//li[contains(@class, 'toggle')]").ToList();

            foreach (var product in products)
            {
                var inStock = product.SelectSingleNode(".//div[contains(@class, 'site-stock-text hidden-xs')]");
                if (inStock != null && !string.IsNullOrEmpty(inStock.InnerText))
                {
                    var decode = HttpUtility.HtmlDecode(inStock.InnerText);
                    var countN = 0;

                    if (decode.Contains("dager til levering"))
                    {
                        var productLink = product.SelectSingleNode(".//a[contains(@class, 'site-product-link')]");
                        if (productLink != null)
                        {
                            var hrefValue = productLink.GetAttributeValue("href", string.Empty);
                            countN = await GetProshopCount("https://www.proshop.no" + hrefValue);

                            list.Add(new InStockItem { Url = "https://www.proshop.no" + hrefValue, Name = item.Name, Count = countN, Channel = item.DiscordChannel, Store = "Proshop.no",  ChannelId = item.DiscordChannelId });
                        }
                    }
                }
            }

            return list;
        }

        private async Task<int> GetProshopCount(string url)
        {
            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(url, Encoding.UTF8, CancellationToken.None);
            var products = doc.DocumentNode.SelectSingleNode(".//b[contains(@class, 'site-stock pull-right')]");

            var regexNumber = new Regex(@"[0-9]{1,3}");
            var countS = regexNumber.Match(products.InnerText)?.Value;
            var countN = 0;

            if (!string.IsNullOrEmpty(countS)) _ = int.TryParse(countS, out countN);
            return countN;
        }
    }
}
