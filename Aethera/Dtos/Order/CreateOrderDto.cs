namespace Aethera.Dtos.Order
{
    public class ShippingAddressDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class CreateOrderDto
    {
        public ShippingAddressDto ShippingAddress { get; set; } = new();
    }
}
