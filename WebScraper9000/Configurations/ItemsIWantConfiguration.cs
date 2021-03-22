using System.Collections.Generic;

namespace WebScraper9000.Configurations
{
    public class ItemsIWantConfiguration
    {
        public List<ItemsIWant> Items { get; set; }
    }

    public partial class ItemsIWant
    {
        public string Name { get; set; }
        public string KomplettUrl { get; set; }
        public string ElkjopUrl { get; set; }
        public string DiscordChannel { get; set; }
    }
}
