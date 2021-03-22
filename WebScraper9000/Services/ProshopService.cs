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
    public class ProshopService : IProshopService
    {
        public async Task<List<InStockItem>> GetItemInStockFromProshop(string url, string name, string discordChannel)
        {
            var list = new List<InStockItem>();

            var webCrawler = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var doc = await webCrawler.LoadFromWebAsync(url, Encoding.UTF8, CancellationToken.None);

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
                            list.Add(new InStockItem { Url = "https://www.proshop.no/" + hrefValue, Name = name, Count = countN, Channel = discordChannel, Store = "Proshop.no" });
                        }
                    }
                }
            }

            return list;
        }

    }
}