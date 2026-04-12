using Aethera.Dtos.Order;
using Aethera.Interfaces;
using Aethera.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aethera.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IGenericRepository<Models.Order> _orderRepository;
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IMapper _mapper;

        public OrderController(
            IGenericRepository<Models.Order> orderRepository,
            IGenericRepository<Cart> cartRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new Exception("User ID not found in claims");

        // POST /api/order — checkout: converts cart to order, clears cart
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto dto)
        {
            var userId = GetUserId();

            var cart = await _cartRepository.GetByIdAsync(userId, userId);
            if (cart == null || cart.Items.Count == 0)
                return BadRequest(new { message = "Your cart is empty." });

            var order = new Models.Order
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Items = cart.Items.ToList(),
                ShippingAddress = _mapper.Map<ShippingAddress>(dto.ShippingAddress),
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _orderRepository.AddAsync(order);
            await _cartRepository.DeleteAsync(userId, userId);

            var result = _mapper.Map<OrderDto>(created);
            result.TotalPrice = created.TotalPrice;

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        // GET /api/order — user's order history
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _orderRepository.FindAsync(o => o.UserId == userId);
            var result = orders.Select(o =>
            {
                var dto = _mapper.Map<OrderDto>(o);
                dto.TotalPrice = o.TotalPrice;
                return dto;
            });
            return Ok(result);
        }

        // GET /api/order/{id} — order detail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var userId = GetUserId();
            var order = await _orderRepository.GetByIdAsync(id, userId);
            if (order == null || order.UserId != userId)
                return NotFound();

            var result = _mapper.Map<OrderDto>(order);
            result.TotalPrice = order.TotalPrice;
            return Ok(result);
        }

        // PUT /api/order/{id}/cancel — user cancels if still Pending
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
        {
            var userId = GetUserId();
            var order = await _orderRepository.GetByIdAsync(id, userId);
            if (order == null || order.UserId != userId)
                return NotFound();

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new { message = "Only pending orders can be cancelled." });

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order, userId);

            return NoContent();
        }

        // PUT /api/order/{id}/status — admin updates order status
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _orderRepository.FindAsync(o => o.Id == id);
            var target = order.FirstOrDefault();
            if (target == null)
                return NotFound();

            target.Status = dto.Status;
            target.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(target, target.UserId);

            return NoContent();
        }
    }
}
