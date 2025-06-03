using Aethera.Interfaces;
using System.Text.Json.Serialization;

namespace Aethera.Models
{
    public class Product : ICosmosEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("partitionKey")]
        public string PartitionKey => Category; 
        [JsonIgnore]
        public string ContainerName => "ProductContainer";
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = "perfumes";
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

    }
}

