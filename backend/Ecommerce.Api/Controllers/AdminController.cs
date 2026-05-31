using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Ecommerce.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AdminKey]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            _logger.LogInformation("GetUsers called");
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, "Error retrieving users");
            }
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult<User>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updated = await _adminService.UpdateUserAsync(id, dto.Name, dto.Role);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var success = await _adminService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats()
        {
            var stats = await _adminService.GetStatsAsync();
            return Ok(stats);
        }

        [AllowAnonymous]
        [HttpPost("track-view/{productId}")]
        public IActionResult TrackProductView(Guid productId)
        {
            _adminService.TrackProductView(productId);
            return Ok($"Product {productId} view counted.");
        }
    }

    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
