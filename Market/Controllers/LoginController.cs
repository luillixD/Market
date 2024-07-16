using AutoMapper;
using Market.DTOs.Login;
using Market.DTOs.Users;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public LoginController(ILoginService loginService, IMapper mapper, ILogger<UserController> logger)
        {
            _loginService = loginService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                var user = _mapper.Map<User>(request);
                var createdUser = await _loginService.Register(user);

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

        private object Get()
        {
            throw new NotImplementedException();
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationDto request)
        {
            try
            {
                var token = await _loginService.Authenticate(request.Username, request.Password);
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

        [HttpPost("validate-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateEmail([FromBody] string codeValidation)
        {
            try
            {
                var result = await _loginService.ValidateEmail(codeValidation);
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

        [HttpPost("forget-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto request)
        {
            try
            {
                var result = await _loginService.ForgetPassword(request.UserId, request.NewPassword);
                if (!result)
                    return BadRequest("Error updating password");
                return Ok("Password updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password");
                return StatusCode(500, "An error occurred while updating the password");
            }
        }
    }
}
