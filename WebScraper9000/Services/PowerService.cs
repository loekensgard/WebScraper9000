using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class PowerService : IPowerService
    {
        public async Task<List<InStockItem>> GetItemInStockFromPower(string url, string name, string discordChannel)
        {
            var list = new List<InStockItem>();

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(url, Encoding.UTF8, CancellationToken.None);

            var angularfuckery = doc.DocumentNode.SelectSingleNode(".//power-meta[contains(@id, 'angular-page-model')]");
            var data = angularfuckery.GetAttributeValue("data", null);

            if(data != null)
            {
                var decodedByteHtml = Convert.FromBase64String(data);
                var body = JsonSerializer.Deserialize<PowerProducts>(decodedByteHtml);

                if(body != null && body.Model != null && body.Model.ProductWrapper != null && body.Model.ProductWrapper.Products.Length > 0)
                {
                    foreach(var product in body.Model.ProductWrapper.Products)
                    {
                        if(product.StockCount > 0 && product.CategoryId == 1929)
                        {
                            list.Add(new InStockItem { Url = "https://power.no" + product.Url, Name = name, Count = product.StockCount.Value, Channel = discordChannel, Store = body.Model.SiteName });
                        }
                    }
                }
            }
            return list;
        }

    }
}
