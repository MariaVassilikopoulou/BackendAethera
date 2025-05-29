using Aethera.Dtos;
using Aethera.Interfaces;
using Aethera.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aethera.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IMapper _mapper;
        public ProductController(IGenericRepository<Product> repository, IMapper mapper)
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
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _repository.GetByIdAsync(id, "perfumes");
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var created = await _repository.AddAsync(product);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDto dto)
        {
            var existing = await _repository.GetByIdAsync(id, "perfumes");
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);

            var updated = await _repository.UpdateAsync(existing, existing.PartitionKey);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _repository.DeleteAsync(id, "perfumes");
            return deleted ? NoContent() : NotFound();
        }
    }
}

