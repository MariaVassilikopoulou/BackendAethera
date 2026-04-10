using Aethera.Models;

namespace Aethera.Dtos.Order
{
    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
        public ShippingAddressDto ShippingAddress { get; set; } = new();
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
