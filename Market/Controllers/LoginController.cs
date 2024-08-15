using AutoMapper;
using Market.DTOs.Login;
using Market.DTOs.Users;
using Market.Models;
using Market.Services;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public LoginController(ILoginService loginService, IMapper mapper, ILogger<UserController> logger, IUserService userService)
        {
            _loginService = loginService;
            _userService = (UserService)userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var user = _mapper.Map<User>(registerDto);
                var createdUser = await _loginService.Register(user);

                var userDto = _mapper.Map<UserDto>(createdUser);
#if !DEBUG
                return StatusCode(201, userDto);
#endif
                return StatusCode(201, user.ValidationCode);
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

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isVerfied = await _loginService.IsVerifiedUser(request.Email);
                if (!isVerfied) return Unauthorized("User not verified");

                var token = await _loginService.Authenticate(request.Email, request.Password);
                if (string.IsNullOrWhiteSpace(token))
                    return Unauthorized("Invalid email or password");
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
                return StatusCode(500, "An error occurred during authentication");
            }
        }

        [HttpPost("forget-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _loginService.ForgetPassword(request.Email, request.NewPassword);
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

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto body)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userEmail)) return BadRequest("Invalid token");

                var result = await _loginService.ChangePassword(userEmail, body.OldPassword, body.NewPassword);
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

        [HttpPost("validate-email/{codeValidation}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateEmail(string codeValidation)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codeValidation)) return BadRequest("Validation code is required");
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
    }
}
