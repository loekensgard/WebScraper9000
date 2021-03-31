using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
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
		private readonly IEnumerable<IStoreService> _storeServices;
		private readonly DiscordConfiguration _discordOptions;
		private readonly ItemsIWantConfiguration _options;

		public WebScraper(
			IDiscordService discordService,
			IEnumerable<IStoreService> storeServices,
			IOptions<ItemsIWantConfiguration> options,
			IOptions<DiscordConfiguration> discordOptions)
		{
			_discordService = discordService;
			_storeServices = storeServices;
			_discordOptions = discordOptions.Value;
			_options = options.Value;
		}

		[FunctionName("WebScraper")]
		public async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer, ILogger log)
		{
			log.LogInformation("Worker running at: {time}", DateTime.Now.ToString("d/MM/yy HH:mm"));
			if (_options.Items == null) log.LogError("No items loaded from settings");

			await _discordService.UpdateDiscordMessages(_options.Items);

			var tasks = new List<Task<IEnumerable<InStockItem>>>();
			var discordAlertTasks = new List<Task>();

			if (_options.Items != null)
			{
				try
				{
					log.LogInformation("Checking {count} items", _options.Items.Count);
					foreach (var item in _options.Items)
					{
						var linked = !string.IsNullOrEmpty(item.DiscordChannel);
						log.LogInformation("Checking {name} and discord channel is {linked} with {id}", item.Name, linked, item.DiscordChannelId);

						foreach (var store in _storeServices)
						{
							var task = store.GetItemInStock(item);
							discordAlertTasks.Add(AlertDiscord(log, task));
							tasks.Add(task);
						}
					}
				}
				catch (Exception e)
				{
					log.LogError(e, "Failed getting status");
					await _discordService.SendError(_discordOptions.ErrorChannel, e.Message);
				}
			}

			var list = (await Task.WhenAll(tasks)).SelectMany(result => result);

			var outOfStock = _options.Items.Where(item => !list.Where(i => i.Name == item.Name).Any());

			await SendOutOfStock(log, outOfStock);
			await Task.WhenAll(discordAlertTasks);
		}

		private async Task SendOutOfStock(ILogger log, IEnumerable<ItemsIWant> outOfStock)
		{
			foreach (var item in outOfStock)
			{
				try
				{
					await _discordService.SendNoItems(item.DiscordChannel, item.DiscordChannelId);
					log.LogInformation("Found no items for {item} at {time}", item.Name, DateTime.Now.ToString("d/MM/yy HH:mm"));
				}
				catch (Exception e)
				{
					log.LogError(e, "Failed sending discord message");
					await _discordService.SendError(_discordOptions.ErrorChannel, e.Message);
				}
			}
		}

		private async Task AlertDiscord(ILogger log, Task<IEnumerable<InStockItem>> task)
		{
			var list = await task;

			if (list.Any())
			{
				try
				{
					await _discordService.SendDiscordMessage(list);
				}
				catch (Exception e)
				{
					log.LogError(e, "Failed sending discord message for {item}", list.FirstOrDefault().Name);
					await _discordService.SendError(_discordOptions.ErrorChannel, e.Message);
				}
			}
		}
	}
}
