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
    }
}