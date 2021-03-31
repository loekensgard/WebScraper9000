using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebScraper9000.Configurations;
using WebScraper9000.Models;

namespace WebScraper9000.Interfaces
{
	public interface IStoreService
	{
		Task<IEnumerable<InStockItem>> GetItemInStock(ItemsIWant item);
	}
}
