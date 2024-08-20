using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Services.Interfaces;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Market.DTOs.Users;
using System.Security.Claims;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IMapper mapper, ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            if (id <= 0) return BadRequest("Invalid id");

            try
            {
                var user = await _userService.Get(id);
                var role = await _userService.GetUserRoles(id);
                UserWithRoleDto userWithRoleDto = new UserWithRoleDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = role
                };

                if (user == null)
                    return NotFound();
                return Ok(userWithRoleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with id {id}");
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var user = await _userService.Get(userId);
                if (user == null)
                    return NotFound();
                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with id {userId}");
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        [HttpGet("/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) return BadRequest("Invalid page number");
            try
            {
                var users = await _userService.GetAll(page, pageSize);
                return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto body)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (body == null) return BadRequest("User data is required");
            if (string.IsNullOrEmpty(body.FullName) && string.IsNullOrEmpty(body.Email) && string.IsNullOrEmpty(body.PhoneNumber)) return BadRequest("At least one field is required");

            try
            {
                var updatedUser = await _userService.Update(userId, body.FullName, body.Email, body.PhoneNumber);
                if (updatedUser == null)
                    return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with id {userId}");
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        [HttpDelete("/Delete")]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _userService.Delete(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id {userId}");
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }

        [HttpDelete("/Delete{id}")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id {id}");
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }
    }
}