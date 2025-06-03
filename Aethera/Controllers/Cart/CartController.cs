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
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found");

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            var cart = await _repository.GetByIdAsync(userId, userId);
            return cart is null ? NotFound() : Ok(cart);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CartDto dto)
        {
            var cart = _mapper.Map<Cart>(dto);
            cart.Id = cart.UserId; // if needed
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


        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCart([FromBody] CartDto dto)
        {
            var cart = _mapper.Map<Cart>(dto);
            var result = await _repository.UpsertAsync(cart);
            return Ok(result);
        }


        [HttpGet("me/cart")]
        public async Task<IActionResult> GetMyCart()
        {
            string userId = User.GetUserId(); // Gets the current user's ID
            var cart = await _repository.GetByIdAsync(userId, userId);
            return Ok(cart);
        }
    }
}
