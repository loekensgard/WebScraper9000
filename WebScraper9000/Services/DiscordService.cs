using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebScraper9000.Configurations;
using WebScraper9000.Exceptions;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000.Services
{
    public class DiscordService : IDiscordService
    {
        private const string OUT_OF_STOCK_MESSAGE = "**Out of stock**";
        private readonly HttpClient _httpClient;
        private readonly HttpResponseService _httpResponseService;
        private IDictionary<string, IEnumerable<DiscordMessage>> discordMessages;

        public DiscordService(HttpClient httpClient, HttpResponseService httpResponseService)
        {
            _httpClient = httpClient;
            _httpResponseService = httpResponseService;
            discordMessages = new Dictionary<string, IEnumerable<DiscordMessage>>();
        }

        public async Task SendDiscordMessage(IEnumerable<InStockItem> list)
        {
            foreach (var item in list)
            {
                var x = "Ukjent antall";
                if (item.Count != 0)
                    x = item.Count.ToString();

                var message = $"**{x}** {item.Name} på lager hos **{item.Store}**: {item.Url}";

                bool alreadyPosted = false;

                if (discordMessages.TryGetValue(item.ChannelId, out var messages))
                {
                    alreadyPosted = messages.Any(m => m.Content == message) && messages.First().Content != OUT_OF_STOCK_MESSAGE;
                }

                if (!alreadyPosted)
                {
                    var body = new { username = "GrabIt", content = message };
                    var response = await _httpClient.PostAsJsonAsync(item.Channel, body);

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task SendError(string channel, string error)
        {
            var body = new { username = "WebScraper9000", content = error };
            var response = await _httpClient.PostAsJsonAsync(channel, body);
        }

        public async Task SendNoItems(string channel, string channelId)
        {
            bool alreadyPosted = true;

            if (discordMessages.TryGetValue(channelId, out var messages))
            {
                if (!messages.Any())
                    alreadyPosted = false;
                else
                    alreadyPosted = messages.First().Content == OUT_OF_STOCK_MESSAGE;
            }

            if (!alreadyPosted)
            {
                var body = new { username = "GrabIt", content = OUT_OF_STOCK_MESSAGE };
                var response = await _httpClient.PostAsJsonAsync(channel, body);

                response.EnsureSuccessStatusCode();
            }
        }

        public async Task UpdateDiscordMessages(IEnumerable<ItemsIWant> items)
        {
            foreach (var item in items)
            {
                try
                {
                    discordMessages.Add(item.DiscordChannelId, await GetChannelMessages(item.DiscordChannelId));
                }
                catch (Exception)
                {
                    // idk
                }
            }
        }

        private async Task<IEnumerable<DiscordMessage>> GetChannelMessages(string channelId)
        {
            var response = await _httpClient.GetAsync($"api/v8/channels/{channelId}/messages?limit={10}");
            return await response.Content.ReadAsAsync<IEnumerable<DiscordMessage>>();
        }
    }
}
