using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class DiscordService : IDiscordService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpResponseService _httpResponseService;

        public DiscordService(HttpClient httpClient, HttpResponseService httpResponseService)
        {
            _httpClient = httpClient;
            _httpResponseService = httpResponseService;
        }
        public async Task SendDiscordMessage(List<InStockItem> list)
        {
            foreach (var item in list)
            {
                var x = "Ukjent antall";
                if (item.Count != 0)
                    x = item.Count.ToString();

                var message = $"**{x}** {item.Name} på lager hos **{item.Store}**: {item.Url}";

                if(!await AlreadyPosted(message, item.ChannelId, "10"))
                {
                    var body = new { username = "GrabIt", content = message };
                    var response = await _httpClient.PostAsJsonAsync(item.Channel, body);

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private async Task<bool> AlreadyPosted(string message, string channelId, string count)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v8/channels/{channelId}/messages?limit={count}");
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                var messages = await _httpResponseService.DeserializeJsonFromStream<List<DiscordMessage>>(response);
                if (messages.Any(x => x.Content == message)) return true;
            }

            return false;
        }

        public async Task SendError(string channel, string error)
        {
            var body = new { username = "WebScraper9000", content = error };
            var response = await _httpClient.PostAsJsonAsync(channel, body);
        }

        public async Task SendNoItems(string channel, string channelid)
        {
            var message = "**Out of stock**";

            if (!await AlreadyPosted(message, channelid, "1"))
            {
                var body = new { username = "GrabIt", content = message };
                var response = await _httpClient.PostAsJsonAsync(channel, body);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
