using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using WebScraper9000.Configurations;
using WebScraper9000.Interfaces;
using WebScraper9000.Services;

[assembly: FunctionsStartup(typeof(WebScraper9000.Startup))]

namespace WebScraper9000
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var localConfig = new ConfigurationBuilder()
                .SetBasePath(GetBasePath(builder))
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<ItemsIWantConfiguration>(options => localConfig.GetSection("ItemsIWant").Bind(options));
            builder.Services.Configure<DiscordConfiguration>(options => localConfig.GetSection("Discord").Bind(options));

            builder.Services.AddTransient<HttpResponseService>();
            builder.Services.AddHttpClient<IDiscordService, DiscordService>()
                        .ConfigureHttpClient(c =>
                        {
                            c.BaseAddress = new Uri("https://discord.com/");
                            c.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bot", localConfig.GetSection("Discord")["Token"]);
                            c.DefaultRequestHeaders.Add("User-Agent", "DiscordBot C#");
                        }
                        );
            builder.Services.AddSingleton<IStoreService, KomplettService>();
            builder.Services.AddSingleton<IStoreService, ElkjopService>();
            builder.Services.AddSingleton<IStoreService, ProshopService>();
            builder.Services.AddSingleton<IStoreService, PowerService>();
            builder.Services.AddSingleton<IStoreService, MulticomService>();
            builder.Services.AddSingleton<IStoreService, NetonnetService>();
            builder.Services.AddSingleton<IStoreService, PreBuiltKomplett>();
        }

        private string GetBasePath(IFunctionsHostBuilder builder)
        {
            var executioncontextoptions = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            return executioncontextoptions.AppDirectory;
        }
    }
}
