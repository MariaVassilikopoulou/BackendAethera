using Aethera.Authentication;
using Aethera.Dtos;
using Aethera.Interfaces;
using Aethera.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aethera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IGenericRepository<Cart> _repository;
        private readonly IMapper _mapper;

        public CartController(IGenericRepository<Cart> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new Exception("User ID not found in claims");

       
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetUserId();
            var cart = await _repository.GetByIdAsync(userId, userId);
            if (cart == null)
            {
                
                return Ok(new Cart { UserId = userId, Id = userId, Items = new List<CartItem>() });
            }
            return Ok(cart);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCart([FromBody] CartDto dto)
        {
            try
            {
               
                var userId = GetUserId();

                
                if (dto.Items == null)
                    dto.Items = new List<CartItemDto>();

                
                var cart = _mapper.Map<Cart>(dto);

                
                cart.UserId = userId;
                cart.Id = userId;
                cart.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"[CartController] Saving cart for user {userId}");
                Console.WriteLine($"Cart JSON: {System.Text.Json.JsonSerializer.Serialize(cart)}");

                
                var result = await _repository.UpsertAsync(cart);

                return Ok(result);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("[CartController] Error saving cart:");
                Console.WriteLine(ex.ToString());

                
                return StatusCode(500, new
                {
                    message = "Backend error: 500",
                    details = ex.Message
                });
            }
        }


        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CartDto dto)
        {
            var userId = GetUserId();

            var cart = _mapper.Map<Cart>(dto);
            cart.UserId = userId;
            cart.Id = userId;
            cart.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpsertAsync(cart);
            return Ok(updated);
        }

        
        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            var deleted = await _repository.DeleteAsync(userId, userId);
            return deleted ? NoContent() : NotFound();
        }
    }
}
