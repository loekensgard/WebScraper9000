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
    public class ElkjopService : IStoreService
    {
        public async Task<IEnumerable<InStockItem>> GetItemInStock(ItemsIWant item)
        {
            var list = new List<InStockItem>();

            if (string.IsNullOrEmpty(item.ElkjopUrl))
			{
                return list;
			}

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(item.ElkjopUrl, Encoding.UTF8, CancellationToken.None);

            var products = doc.DocumentNode.Descendants().Where(x => x.HasClass("mini-product")).ToList();

            foreach (var product in products)
            {
                var inStock = product.Descendants().FirstOrDefault(x => x.HasClass("checkout-spacing"));
                if (inStock != null && !string.IsNullOrEmpty(inStock.InnerText))
                {
                    var decode = HttpUtility.HtmlDecode(inStock.InnerText);
                    var countN = GetCountInStock(decode);

                    var productLink = product.Descendants().FirstOrDefault(x => x.HasClass("product-image-link"));
                    if (productLink != null)
                    {
                        var hrefValue = productLink.GetAttributeValue("href", string.Empty);
                        list.Add(new InStockItem { Url = hrefValue, Name = item.Name, Count = countN, Channel = item.DiscordChannel, Store = "Elkjop.no", ChannelId = item.DiscordChannelId });
                    }
                }
            }

            return list;
        }

        private static int GetCountInStock(string count)
        {
            var regexNumber = new Regex(@"[0-9]{1,3}");
            var countS = regexNumber.Match(count)?.Value;
            var countN = 0;

            if (!string.IsNullOrEmpty(countS)) _ = int.TryParse(countS, out countN);
            return countN;
        }
    }
}
