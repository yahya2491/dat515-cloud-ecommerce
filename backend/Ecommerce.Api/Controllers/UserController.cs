using System.Collections.Generic;
using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            _logger.LogInformation("GetAll users called");
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, "Error retrieving users");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromHeader] string username, [FromHeader] string nickname, [FromHeader] string password)
        {
            _logger.LogInformation("Register called for username={Username}", username);
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return BadRequest("Username and password are required.");

                if (nickname == null)
                    return BadRequest("Nickname cannot be null.");

                var user = await _userService.RegisterAsync(username, nickname, password);
                if (user == null)
                {
                    _logger.LogWarning("Register conflict - username exists: {Username}", username);
                    return Conflict("Username already exists.");
                }

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", username);
                return StatusCode(500, "Error registering user");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromHeader] string username, [FromHeader] string password)
        {
            _logger.LogInformation("Login attempt for username={Username}", username);
            try
            {
                var user = await _userService.LoginAsync(username, password);
                if (user == null)
                {
                    _logger.LogWarning("Unauthorized login attempt for {Username}", username);
                    return Unauthorized("Invalid username or password.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Username}", username);
                return StatusCode(500, "Error logging in");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            _logger.LogInformation("GetById user {UserId} called", id);
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", id);
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return StatusCode(500, "Error retrieving user");
            }
        }
    }
}
