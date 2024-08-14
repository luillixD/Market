using Market.DTOs.Subcategory;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoryController : ControllerBase
    {
        private readonly ISubcategoryService _service;
        private readonly ILogger<SubcategoryController> _logger;

        public SubcategoryController(ISubcategoryService service, ILogger<SubcategoryController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubcategoryDto subcategoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var subcategory = await _service.Create(subcategoryDto);
                return CreatedAtAction(nameof(GetById), new { id = subcategory.Id }, subcategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subcategory");
                return StatusCode(500, "An error occurred while creating the subcategory");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSubcategoryDto subcategoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var subcategory = await _service.Update(id, subcategoryDto);
                return Ok(subcategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subcategory");
                return StatusCode(500, "An error occurred while updating the subcategory");
            }
        }

        [HttpDelete("{id}")]
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
                _logger.LogError(ex, "Error deleting subcategory");
                return StatusCode(500, "An error occurred while deleting the subcategory");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var subcategory = await _service.GetById(id);
            if (subcategory == null) return NotFound();
            return Ok(subcategory);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subcategories = await _service.GetAll();
            return Ok(subcategories);
        }

    }
}
