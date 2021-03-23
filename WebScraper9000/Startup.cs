using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
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

            builder.Services.AddHttpClient<IDiscordService, DiscordService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://discord.com/"));
            builder.Services.AddSingleton<IKomplettService, KomplettService>();
            builder.Services.AddSingleton<IElkjopService, ElkjopService>();
            builder.Services.AddSingleton<IProshopService, ProshopService>();
            builder.Services.AddSingleton<IPowerService, PowerService>();
            builder.Services.AddSingleton<IMulticomService, MulticomService>();
        }

        private string GetBasePath(IFunctionsHostBuilder builder)
        {
            var executioncontextoptions = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            return executioncontextoptions.AppDirectory;
        }
    }
}
