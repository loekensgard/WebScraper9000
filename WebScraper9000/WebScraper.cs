using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebScraper9000.Configurations;
using WebScraper9000.Interfaces;
using WebScraper9000.Models;

namespace WebScraper9000
{
    public class WebScraper
    {
        private readonly IDiscordService _discordService;
        private readonly IKomplettService _komplettService;
        private readonly IElkjopService _powerService;
        private readonly ItemsIWantConfiguration _options;

        public WebScraper(IDiscordService discordService, IKomplettService komplettService, IElkjopService elkjopService, IOptions<ItemsIWantConfiguration> options)
        {
            _discordService = discordService;
            _komplettService = komplettService;
            _powerService = elkjopService;
            _options = options.Value;
        }

        [FunctionName("WebScraper")]
        public async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Worker running at: {time}", DateTime.Now.ToString("d/MM/yy HH:mm"));
            if (_options.Items == null) log.LogError("No items loaded from settings");

            var list = new List<InStockItem>();

            if(_options.Items != null)
            {
                log.LogInformation("Checking {count} items", _options.Items.Count);
                foreach (var item in _options.Items)
                {
                    if(!string.IsNullOrEmpty(item.KomplettUrl))
                        list.AddRange(await _komplettService.GetItemInStockFromKomplett(item.KomplettUrl, item.Name, item.DiscordChannel));
                    if(!string.IsNullOrEmpty(item.ElkjopUrl))
                        list.AddRange(await _powerService.GetItemInStockFromPower(item.ElkjopUrl, item.Name, item.DiscordChannel));
                }
            }

            if (list.Any())
            {
                try
                {
                    await _discordService.SendDiscordMessage(list);
                }catch(Exception e)
                {
                    log.LogError(e, "Failed sending discord message");
                }
            }
            else
            {
                log.LogInformation("Found no items at {time}", DateTime.Now.ToString("d/MM/yy HH:mm"));
            }
        }
    }
}
