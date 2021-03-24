using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class NetonnetService : INetonnetService
    {
        public async Task<List<InStockItem>> GetItemInStockFromNetonnet(string url, string name, string discordChannel, string channelId)
        {
            var list = new List<InStockItem>();

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(url, Encoding.UTF8, CancellationToken.None);

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
                        list.Add(new InStockItem { Url = "https://netonnet.no" + hrefValue, Name = name, Count = 0, Channel = discordChannel, Store = "NetOnNet.no", ChannelId = channelId });
                    }
                }
            }

            return list;
        }
    }
}
