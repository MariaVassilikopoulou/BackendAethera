using Aethera.Interfaces;
using System.Text.Json.Serialization;

namespace Aethera.Models
{
    public class Cart : ICosmosEntity
    {
        
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;


        [JsonPropertyName("partitionkey")]
        public string PartitionKey => UserId;

       
        [JsonIgnore]
        public string ContainerName => "CartContainer";

       
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

       
        [JsonPropertyName("items")]
        public List<CartItem> Items { get; set; } = new();

       
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

       
        [JsonIgnore]
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    }
}

