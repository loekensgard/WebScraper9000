using System.Text.Json.Serialization;

namespace WebScraper9000.Models
{
    public partial class DiscordMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonPropertyName("author")]
        public Author Author { get; set; }
    }

    public partial class Author
    {
        [JsonPropertyName("bot")]
        public bool Bot { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
