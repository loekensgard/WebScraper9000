﻿using HtmlAgilityPack;
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
    public class KomplettService : IKomplettService
    {
        public async Task<List<InStockItem>> GetItemInStockFromKomplett(string url, string name, string discordChannel)
        {
            var list = new List<InStockItem>();

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(url, Encoding.UTF8, CancellationToken.None);

            var products = doc.DocumentNode.Descendants().Where(x => x.HasClass("product-list-item")).ToList();

            foreach (var product in products)
            {
                var inStock = product.Descendants().FirstOrDefault(x => x.HasClass("stockstatus-stock-details"));
                if (inStock != null && !string.IsNullOrEmpty(inStock.InnerText))
                {
                    var decode = HttpUtility.HtmlDecode(inStock.InnerText);
                    var countN = GetCountInStock(decode);

                    var regex = new Regex(@"(?:^|\W)på lager.$(?:$|\W)");
                    if (regex.IsMatch(decode))
                    {
                        var productLink = product.Descendants().FirstOrDefault(x => x.HasClass("product-link"));
                        if (productLink != null)
                        {
                            var hrefValue = productLink.GetAttributeValue("href", string.Empty);
                            list.Add(new InStockItem { Url = "https://komplett.no" + hrefValue, Name = name, Count = countN, Channel = discordChannel, Store = "Komplett.no" });
                        }
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
