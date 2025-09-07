//using Aethera.Interfaces;
//using System.Text.Json.Serialization;

//namespace Aethera.Models
//{
//    public class Cart : ICosmosEntity
//    {
//        [JsonPropertyName("id")]
//        public string Id { get; set; } = Guid.NewGuid().ToString();

//        [JsonPropertyName("partitionkey")]
//        public string PartitionKey => UserId;

//        [JsonIgnore]
//        public string ContainerName => "CartContainer";

//        [JsonPropertyName("userId")]
//        public string UserId { get; set; } = string.Empty;

//        [JsonPropertyName("items")]
//        public List<CartItem> Items { get; set; } = new();

//        [JsonPropertyName("createdAt")]
//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//        [JsonPropertyName("updatedAt")]
//        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
//        [JsonIgnore]
//        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);


//    }
//}   


using Aethera.Interfaces;
using System.Text.Json.Serialization;

namespace Aethera.Models
{
    public class Cart : ICosmosEntity
    {
        // Cosmos DB requires "id" field
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        // Partition key MUST match how you created the container: /partitionkey
        [JsonPropertyName("partitionkey")]
        public string PartitionKey => UserId;

        // The container where this entity is stored
        [JsonIgnore]
        public string ContainerName => "CartContainer";

        // User who owns this cart
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        // Items in the cart
        [JsonPropertyName("items")]
        public List<CartItem> Items { get; set; } = new();

        // Metadata
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Not stored in Cosmos (calculated only in C#)
        [JsonIgnore]
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    }
}

