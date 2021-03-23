using System;
using System.Text.Json.Serialization;

namespace WebScraper9000.Models
{
    public partial class PowerProducts
    {
        [JsonPropertyName("DocumentId")]
        public long? DocumentId { get; set; }

        [JsonPropertyName("Model")]
        public Model Model { get; set; }
    }

    public partial class Model
    {
        [JsonPropertyName("ProductWrapper")]
        public ProductWrapper ProductWrapper { get; set; }

        [JsonPropertyName("SiteName")]
        public string SiteName { get; set; }

    }

    public partial class ProductWrapper
    {
        [JsonPropertyName("Products")]
        public Product[] Products { get; set; }
    }

    public partial class Product
    {
        [JsonPropertyName("PreviousPrice")]
        public object PreviousPrice { get; set; }

        [JsonPropertyName("PreviousPriceDisclaimer")]
        public object PreviousPriceDisclaimer { get; set; }

        [JsonPropertyName("CategoryId")]
        public int? CategoryId { get; set; }

        //[JsonPropertyName("Price")]
        //public long? Price { get; set; }

        [JsonPropertyName("ShortDescription")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("StockCount")]
        public int? StockCount { get; set; }

        [JsonPropertyName("StockDeliveryDate")]
        public DateTimeOffset? StockDeliveryDate { get; set; }

        [JsonPropertyName("StockDeliveryDateConfirmed")]
        public bool StockDeliveryDateConfirmed { get; set; }

        [JsonPropertyName("StockLimitedRemaining")]
        public long? StockLimitedRemaining { get; set; }

        [JsonPropertyName("StockText")]
        public string StockText { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Url")]
        public string Url { get; set; }

        [JsonPropertyName("TitleSlug")]
        public string TitleSlug { get; set; }

        [JsonPropertyName("TextHeading")]
        public string TextHeading { get; set; }

        [JsonPropertyName("HasUpsaleProduct")]
        public bool HasUpsaleProduct { get; set; }

        [JsonPropertyName("AllowBuyingOutOfStock")]
        public bool AllowBuyingOutOfStock { get; set; }

        [JsonPropertyName("WebStockStatus")]
        public long? WebStockStatus { get; set; }

        [JsonPropertyName("CanAddToCart")]
        public bool CanAddToCart { get; set; }

        [JsonPropertyName("IsOnDemand")]
        public bool IsOnDemand { get; set; }
    }
}
