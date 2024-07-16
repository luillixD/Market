using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Services.Interfaces;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Market.DTOs.Users;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if(createUserDto == null) return BadRequest("User data is required");
                if(string.IsNullOrEmpty(createUserDto.Name)) return BadRequest("Name is required");
                if(string.IsNullOrEmpty(createUserDto.LastName)) return BadRequest("Last Name is required");
                if(string.IsNullOrEmpty(createUserDto.Email)) return BadRequest("Email is required");
                if(string.IsNullOrEmpty(createUserDto.PhoneNumber)) return BadRequest("Phone Number is required");
                if(string.IsNullOrEmpty(createUserDto.Password)) return BadRequest("Password is required");
                if(string.IsNullOrEmpty(createUserDto.ConfirmationPassword)) return BadRequest("Confirmation Password is required");

                if (createUserDto.Password != createUserDto.ConfirmationPassword) return BadRequest("Password and Confirmation Password do not match");

                if(string.IsNullOrEmpty(createUserDto.Role)) createUserDto.Role = "User";

                var user = _mapper.Map<User>(createUserDto);
                var createdUser = await _userService.Create(user, createUserDto.Role);
                var userDto = _mapper.Map<UserDto>(createdUser);
                return CreatedAtAction(nameof(Get), new { id = userDto.Id }, userDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when creating user");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "An error occurred while creating the user");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            try
            {
                var user = await _userService.Get(id);
                if (user == null)
                    return NotFound();
                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with id {id}");
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
        {
            try
            {
                if (id != userDto.Id)
                    return BadRequest();

                var user = _mapper.Map<User>(userDto);
                var updatedUser = await _userService.Update(user);
                if (updatedUser == null)
                    return NotFound();
                return Ok(_mapper.Map<UserDto>(updatedUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with id {id}");
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.Delete(id);
                if (!result)
                    return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id {id}");
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }

        [HttpPost("validate-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateEmail([FromBody] string codeValidation)
        {
            try
            {
                var result = await _userService.ValidateEmail(codeValidation);
                if (!result)
                    return BadRequest("Invalid validation code");
                return Ok("Email validated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating email");
                return StatusCode(500, "An error occurred while validating the email");
            }
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthRequest request)
        {
            try
            {
                var token = await _userService.Authenticate(request.Username, request.Password);
                if (string.IsNullOrEmpty(token))
                    return Unauthorized();
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
                return StatusCode(500, "An error occurred during authentication");
            }
        }
    }

    public class AuthRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}