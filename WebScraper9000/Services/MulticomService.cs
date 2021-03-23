using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class MulticomService : IMulticomService
    {
        public async Task<List<InStockItem>> GetItemInStockFromMulticom(string url, string name, string discordChannel)
        {
            var list = new List<InStockItem>();

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(url, Encoding.UTF8, CancellationToken.None);

            var products = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' b-product-list__item ')]");

            foreach (var product in products)
            {
                var inStock = product.SelectSingleNode(".//span[contains(@class, 'b-stock-info__amount')]");

                if (inStock != null && !string.IsNullOrEmpty(inStock.InnerText))
                {
                    var decode = HttpUtility.HtmlDecode(inStock.InnerText);
                    var countN = GetCountInStock(decode);

                    var productLink = product.SelectSingleNode(".//a");
                    if (productLink != null)
                    {
                        var hrefValue = productLink.GetAttributeValue("href", string.Empty);
                        list.Add(new InStockItem { Url = "https://multicom.no" + hrefValue, Name = name, Count = countN, Channel = discordChannel, Store = "Multicom.no" });
                    }
                }
            }

            return list;
        }

        private static int GetCountInStock(string count)
        {
            var regexNumber = new Regex(@"[0-9]{1,2}");
            var countS = regexNumber.Match(count)?.Value;
            var countN = 0;

            if (!string.IsNullOrEmpty(countS)) _ = int.TryParse(countS, out countN);
            return countN;
        }
    }
}
