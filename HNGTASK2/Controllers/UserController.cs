using HNGTASK2.Data;
using HNGTASK2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HNGTASK2.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public UserController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            try
            {
                // Ensure the authenticated user can access only their own record or records they belong to/created
                var userId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (userId != id)
                {
                    // Unauthorized to access another user's details
                    return Forbid();
                }

                // Call user service to retrieve user details
                var user = await _userService.GetUserDetailsAsync(id);

                if (user == null)
                {
                    return NotFound(new { status = "Not Found", message = "User not found" });
                }

                // Return success response with user details
                return Ok(new
                {
                    status = "success",
                    message = "User details retrieved successfully",
                    data = user
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Internal Server Error", message = ex.Message });
            }
        }
    }
}

