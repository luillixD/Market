using Market.DTOs.Product;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ISubcategoryService _subcategoryService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService service, ISubcategoryService subcategoryService, ILogger<ProductsController> logger)
        {
            _service = service;
            _subcategoryService = subcategoryService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                // Check if the subcategory exists
                var subcategoryExists = await _subcategoryService.ExistsAsync(productDto.SubcategoryId);
                if (!subcategoryExists) return NotFound(new { message = "The specified subcategory does not exist." });
                
                var product = await _service.AddAsync(productDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest("Product ID mismatch");
            }
            await _service.UpdateAsync(productDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }
    }
}
