using Aethera.Models;

namespace Aethera.Dtos
{
    public class UpdateCartDto
    {
        public List<CartItem> Items { get; set; } = new();
    }
}
