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
        private readonly IElkjopService _elkjopService;
        private readonly IProshopService _proshopService;
        private readonly IPowerService _powerService;
        private readonly IMulticomService _multicomService;
        private readonly DiscordConfiguration _discordOptions;
        private readonly ItemsIWantConfiguration _options;

        public WebScraper(
            IDiscordService discordService,
            IKomplettService komplettService, 
            IElkjopService elkjopService, 
            IProshopService proshopService, 
            IPowerService powerService, 
            IMulticomService multicomService ,
            IOptions<ItemsIWantConfiguration> options,
            IOptions<DiscordConfiguration> discordOptions)
        {
            _discordService = discordService;
            _komplettService = komplettService;
            _elkjopService = elkjopService;
            _proshopService = proshopService;
            _powerService = powerService;
            _multicomService = multicomService;
            _discordOptions = discordOptions.Value;
            _options = options.Value;
        }

        [FunctionName("WebScraper")]
        public async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Worker running at: {time}", DateTime.Now.ToString("d/MM/yy HH:mm"));
            if (_options.Items == null) log.LogError("No items loaded from settings");

            var tasks = new List<Task<List<InStockItem>>>();

            if(_options.Items != null)
            {
                try
                {
                    log.LogInformation("Checking {count} items", _options.Items.Count);
                    foreach (var item in _options.Items)
                    {
                        if (!string.IsNullOrEmpty(item.KomplettUrl))
                            tasks.Add(_komplettService.GetItemInStockFromKomplett(item.KomplettUrl, item.Name, item.DiscordChannel, item.DiscordChannelId));
                        if (!string.IsNullOrEmpty(item.ElkjopUrl))
                            tasks.Add(_elkjopService.GetItemInStockFromElkjop(item.ElkjopUrl, item.Name, item.DiscordChannel, item.DiscordChannelId));
                        if (!string.IsNullOrEmpty(item.ProshopUrl))
                            tasks.Add(_proshopService.GetItemInStockFromProshop(item.ProshopUrl, item.Name, item.DiscordChannel, item.DiscordChannelId));
                        if (!string.IsNullOrEmpty(item.MulticomUrl))
                            tasks.Add(_multicomService.GetItemInStockFromMulticom(item.MulticomUrl, item.Name, item.DiscordChannel, item.DiscordChannelId));
                        if (!string.IsNullOrEmpty(item.PowerUrl))
                            tasks.Add(_powerService.GetItemInStockFromPower(item.PowerUrl, item.Name, item.DiscordChannel, item.DiscordChannelId));
                    }
                }
                catch(Exception e)
                {
                    log.LogError(e, "Failed getting status");
                    await _discordService.SendError(_discordOptions.ErrorChannel, e.Message);
                }
            }

            var list = (await Task.WhenAll(tasks)).SelectMany(result => result).ToList();

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
