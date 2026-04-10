using Aethera.Dtos.Product;
using Aethera.Interfaces;
using Aethera.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aethera.Controllers.Products
{
  
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IMapper _mapper;
        public ProductsController(IGenericRepository<Product> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, [FromQuery] string partition = "perfumes")
        {
            var product = await _repository.GetByIdAsync(id, partition);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Add([FromBody] CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var created = await _repository.AddAsync(product);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDto dto, [FromQuery] string partition = "perfumes")
        {
            var existing = await _repository.GetByIdAsync(id, partition);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);

            var updated = await _repository.UpdateAsync(existing, existing.PartitionKey);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string partition = "perfumes")
        {
            var deleted = await _repository.DeleteAsync(id, partition);
            return deleted ? NoContent() : NotFound();
        }
    }
}

