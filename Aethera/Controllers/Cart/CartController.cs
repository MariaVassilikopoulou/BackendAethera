//using Aethera.Authentication;
//using Aethera.Dtos;
//using Aethera.Interfaces;
//using Aethera.Models;
//using AutoMapper;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace Aethera.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class CartController : ControllerBase
//    {
//        private readonly IGenericRepository<Cart> _repository;
//        private readonly IMapper _mapper;
//        public CartController(IGenericRepository<Cart> repository, IMapper mapper)
//        {
//            _repository = repository;
//            _mapper = mapper;
//        }

//        private string GetUserId() =>
//            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found");

//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            var userId = GetUserId();
//            var cart = await _repository.GetByIdAsync(userId, userId);
//            return cart is null ? NotFound() : Ok(cart);
//        }

//        [HttpPut]
//        public async Task<IActionResult> Update([FromBody] CartDto dto)
//        {
//            var cart = _mapper.Map<Cart>(dto);
//            cart.Id = cart.UserId; // if needed
//            var updated = await _repository.UpsertAsync(cart);
//            return Ok(updated);
//        }

//        [HttpDelete]
//        public async Task<IActionResult> Clear()
//        {
//            var userId = GetUserId();
//            var deleted = await _repository.DeleteAsync(userId, userId);
//            return deleted ? NoContent() : NotFound();
//        }


//        [HttpPost]
//        public async Task<IActionResult> AddOrUpdateCart([FromBody] CartDto dto)
//        {
//            var cart = _mapper.Map<Cart>(dto);
//            // Get user ID and log everything
//            var userId = User.GetUserId();
//            Console.WriteLine($"=== DEBUGGING COSMOS PARTITION KEY ===");
//            Console.WriteLine($"Frontend sent UserId: {dto.UserId}");
//            Console.WriteLine($"Backend GetUserId(): {userId}");
//            Console.WriteLine($"Cart.UserId will be: {userId}");
//            Console.WriteLine($"Cart.PartitionKey will be: {userId}"); // Since PartitionKey => UserId
//            Console.WriteLine($"Cart.Id will be: {userId}");

//            cart.UserId = userId;
//            cart.Id = userId;
//            cart.UpdatedAt = DateTime.UtcNow;

//            // Log the final object being sent to Cosmos
//            Console.WriteLine($"Final Cart object: {System.Text.Json.JsonSerializer.Serialize(cart)}");

//            try
//            {
//                var result = await _repository.UpsertAsync(cart);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Cosmos DB Error: {ex.Message}");
//                throw;
//            }
//        }


//        [HttpGet("me/cart")]
//        public async Task<IActionResult> GetMyCart()
//        {
//            string userId = User.GetUserId(); // Gets the current user's ID
//            var cart = await _repository.GetByIdAsync(userId, userId);
//            return Ok(cart);
//        }
//    }
//}


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

        // ✅ Always return current user’s cart
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetUserId();
            var cart = await _repository.GetByIdAsync(userId, userId);
            if (cart == null)
            {
                // Return empty cart object instead of 404 for frontend simplicity
                return Ok(new Cart { UserId = userId, Id = userId, Items = new List<CartItem>() });
            }
            return Ok(cart);
        }

        // ✅ Add or update the current user's cart safely
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCart([FromBody] CartDto dto)
        {
            try
            {
                // Get user ID from token
                var userId = GetUserId();

                // Ensure DTO items list is initialized
                if (dto.Items == null)
                    dto.Items = new List<CartItemDto>();

                // Map DTO to Cart entity
                var cart = _mapper.Map<Cart>(dto);

                // Always use token user ID for PartitionKey and Id
                cart.UserId = userId;
                cart.Id = userId;
                cart.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"[CartController] Saving cart for user {userId}");
                Console.WriteLine($"Cart JSON: {System.Text.Json.JsonSerializer.Serialize(cart)}");

                // Upsert into Cosmos DB
                var result = await _repository.UpsertAsync(cart);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log full exception for debugging
                Console.WriteLine("[CartController] Error saving cart:");
                Console.WriteLine(ex.ToString());

                // Return a detailed but safe error response
                return StatusCode(500, new
                {
                    message = "Backend error: 500",
                    details = ex.Message
                });
            }
        }


        // ✅ Replace cart (PUT = full update)
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

        // ✅ Clear cart
        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            var deleted = await _repository.DeleteAsync(userId, userId);
            return deleted ? NoContent() : NotFound();
        }
    }
}
