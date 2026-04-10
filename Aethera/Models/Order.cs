using Aethera.Interfaces;
using System.Text.Json.Serialization;

namespace Aethera.Models
{
    public enum OrderStatus
    {
        Pending,
        AwaitingPayment,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }

    public class ShippingAddress
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("street")]
        public string Street { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("postcode")]
        public string Postcode { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
    }

    public class Order : ICosmosEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("partitionkey")]
        public string PartitionKey => UserId;

        [JsonIgnore]
        public string ContainerName => "OrderContainer";

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<CartItem> Items { get; set; } = new();

        [JsonPropertyName("shippingAddress")]
        public ShippingAddress ShippingAddress { get; set; } = new();

        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [JsonPropertyName("stripePaymentIntentId")]
        public string? StripePaymentIntentId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    }
}
