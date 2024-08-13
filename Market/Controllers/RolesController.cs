using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Market.DTOs.Roles;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, IMapper mapper, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto createRoleDto)
        {
            if (string.IsNullOrWhiteSpace(createRoleDto.Name)) return BadRequest("Name is required");
            try
            {
                var role = _mapper.Map<Role>(createRoleDto);
                var createdRole = await _roleService.Create(role);
                var roleDto = _mapper.Map<RoleDto>(createdRole);
                return CreatedAtAction(nameof(Get), new { id = roleDto.Id }, roleDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when creating role");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, "An error occurred while creating the role");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> Get(int id)
        {
            if (id <= 0) return BadRequest("Is not valid rol");
            try
            {
                var role = await _roleService.GetById(id);
                if (role == null)
                    return NotFound();
                return Ok(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting role with id {id}");
                return StatusCode(500, "An error occurred while retrieving the role");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            try
            {
                var roles = await _roleService.GetAll();
                return Ok(_mapper.Map<IEnumerable<RoleDto>>(roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return StatusCode(500, "An error occurred while retrieving roles");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            if (id <= 0) return BadRequest("Is not valid rol");
            if (string.IsNullOrWhiteSpace(updateRoleDto.Name)) return BadRequest("Name is required");

            try
            {
                var role = _mapper.Map<Role>(updateRoleDto);
                role.Id = id;
                var updatedRole = await _roleService.Update(role);
                if (updatedRole == null)
                    return NotFound();
                return Ok(_mapper.Map<RoleDto>(updatedRole));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating role with id {id}");
                return StatusCode(500, "An error occurred while updating the role");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Is not valid rol");
            try
            {
                await _roleService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting role with id {id}");
                return StatusCode(500, "An error occurred while deleting the role");
            }
        }
    }
}