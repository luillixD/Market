using Market.DTOs.Product;
using Market.Services;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService service, ILogger<ProductsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var product = await _service.Create(productDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedProduct = await _service.Update(id, productDto);
                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return StatusCode(500, "An error occurred while updating the product");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetById(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAll();
            return Ok(products);
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = null, [FromQuery] decimal? price = null)
        {
            try
            {
                var pagedProducts = await _service.GetPaged(pageNumber, pageSize, orderBy, price);
                return Ok(pagedProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated products");
                return StatusCode(500, "An error occurred while retrieving products");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(string searchText)
        {
            try
            {
                var products = await _service.SearchProducts(searchText);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for products");
                return StatusCode(500, "An error occurred while searching for products");
            }
        }

        [HttpGet("subcategory/{subcategoryId}")]
        public async Task<IActionResult> GetBySubcategory(int subcategoryId)
        {
            var products = await _service.GetBySubcategoryAsync(subcategoryId);
            if (products == null || !products.Any())
            {
                return NotFound("No products found for the specified subcategory.");
            }
            return Ok(products);
        }

    }
}
