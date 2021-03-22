﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebScraper9000.Models;

namespace WebScraper9000.Interfaces
{
    public interface IProshopService
    {
        Task<List<InStockItem>> GetItemInStockFromProshop(string url, string name, string discordChannel);
    }
}