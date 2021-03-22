using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class DiscordService : IDiscordService
    {
        private readonly HttpClient _httpClient;

        public DiscordService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task SendDiscordMessage(List<InStockItem> list)
        {
            foreach (var item in list)
            {
                var body = new { username = "GrabIt", content = $"**{item.Count}** {item.Name} in stock at *{item.Store}*: {item.Url}" };
                var response = await _httpClient.PostAsJsonAsync(item.Channel, body);

                response.EnsureSuccessStatusCode();
            }
        }

    }
}
